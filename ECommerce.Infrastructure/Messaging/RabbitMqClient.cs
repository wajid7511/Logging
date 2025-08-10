using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ECommerce.Infrastructure.Messaging;

public sealed class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string Exchange { get; set; } = "ecommerce.logs";
    public string Queue { get; set; } = "http_logs";
    public string RoutingKey { get; set; } = "http.log";
}

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken);
}

public sealed class RabbitMqClient : IMessagePublisher, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqClient> _logger;

    public RabbitMqClient(IOptions<RabbitMqSettings> options, ILogger<RabbitMqClient> logger)
    {
        _settings = options.Value;
        _logger = logger;
        
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _logger.LogInformation("RabbitMQ Client: Connected to {HostName}", _settings.HostName);
        
        _channel.ExchangeDeclare(_settings.Exchange, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(_settings.Queue, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_settings.Queue, _settings.Exchange, _settings.RoutingKey);
        
        _logger.LogInformation("RabbitMQ Client: Exchange {Exchange}, Queue {Queue}, RoutingKey {RoutingKey} configured", 
            _settings.Exchange, _settings.Queue, _settings.RoutingKey);
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var props = _channel.CreateBasicProperties();
            props.ContentType = "application/json";
            props.DeliveryMode = 2; // persistent
            
            _channel.BasicPublish(exchange: _settings.Exchange, routingKey: _settings.RoutingKey, basicProperties: props, body: body);
            
            _logger.LogInformation("RabbitMQ Client: Message published successfully. Exchange: {Exchange}, RoutingKey: {RoutingKey}, MessageSize: {MessageSize} bytes", 
                _settings.Exchange, _settings.RoutingKey, body.Length);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ Client: Failed to publish message");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}


