using System.Text;
using System.Text.Json;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Messaging;
using ECommerce.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerce.Worker.Consumers;

public sealed class LoggingConsumer(IOptions<RabbitMqSettings> rabbitOptions, IRequestResponseLogRepository logRepository, ILogger<LoggingConsumer> logger) : BackgroundService
{
    private readonly RabbitMqSettings _settings = rabbitOptions.Value;
    private readonly IRequestResponseLogRepository _logRepository = logRepository;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ILogger<LoggingConsumer> _logger = logger;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _logger.LogInformation("Connected to RabbitMQ at {HostName}", _settings.HostName);
            
            _channel.ExchangeDeclare(_settings.Exchange, ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(_settings.Queue, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(_settings.Queue, _settings.Exchange, _settings.RoutingKey);
            
            _logger.LogInformation("Queue {Queue} bound to exchange {Exchange} with routing key {RoutingKey}", 
                _settings.Queue, _settings.Exchange, _settings.RoutingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, ea) =>
            {
                // Use Task.Run to handle async operations properly
                _ = Task.Run(async () =>
                {
                    await _semaphore.WaitAsync(stoppingToken);
                    try
                    {
                        _logger.LogInformation("Received message from RabbitMQ with delivery tag {DeliveryTag}", ea.DeliveryTag);
                        
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        _logger.LogInformation("Message content: {MessageContent}", json);
                        
                        var log = JsonSerializer.Deserialize<RequestResponseLog>(json);
                        if (log != null)
                        {
                            await _logRepository.InsertAsync(log, stoppingToken);
                            _logger.LogInformation("Successfully processed and stored log message for {Method} {Path}", 
                                log.Method, log.Path);
                            
                            // Acknowledge the message only after successful processing
                            _channel.BasicAck(ea.DeliveryTag, multiple: false);
                            _logger.LogInformation("Message acknowledged with delivery tag {DeliveryTag}", ea.DeliveryTag);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to deserialize message, rejecting");
                            _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed processing log message with delivery tag {DeliveryTag}", ea.DeliveryTag);
                        // Reject the message and don't requeue to avoid infinite loops
                        _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }, stoppingToken);
            };

            // Set QoS to process one message at a time
            _channel.BasicQos(0, 1, false);
            
            // Start consuming with autoAck: false to manually control acknowledgments
            _channel.BasicConsume(_settings.Queue, autoAck: false, consumer);
            
            _logger.LogInformation("Started consuming messages from queue {Queue}", _settings.Queue);

            // Keep the service running and wait for cancellation
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoggingConsumer");
            throw;
        }
    }

    public override void Dispose()
    {
        _semaphore?.Dispose();
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}


