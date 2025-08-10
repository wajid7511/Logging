using System.Reflection;
using ECommerce.Infrastructure.Messaging;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddECommerce(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection("Mongo"));
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));

        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        services.AddSingleton<IMessagePublisher, RabbitMqClient>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRequestResponseLogRepository, RequestResponseLogRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("ECommerce.Application")));

        return services;
    }
}


