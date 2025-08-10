using ECommerce.Infrastructure.Messaging;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

// Configure MongoDB settings
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Configure RabbitMQ settings
builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));

// Register MongoDB context
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();

// Register repositories as singletons for hosted services
builder.Services.AddSingleton<IRequestResponseLogRepository, RequestResponseLogRepository>();

// Register RabbitMQ client
builder.Services.AddSingleton<IMessagePublisher, RabbitMqClient>();

// Register the logging consumer as a hosted service
builder.Services.AddHostedService<LoggingConsumer>();

var host = builder.Build();
host.Run();
