using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Persistence;

public sealed class MongoSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "ecommerce";
}

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string name);
}

public sealed class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    static MongoDbContext()
    {
        // Configure entity mappings statically to ensure they're registered before any usage
        ConfigureEntityMappings();
    }

    public MongoDbContext(IOptions<MongoSettings> options)
    {
        // Configure MongoDB conventions
        var conventionPack = new ConventionPack
        {
            new IgnoreIfDefaultConvention(true)
        };
        ConventionRegistry.Register("CustomConventions", conventionPack, t => true);

        // Ensure entity mappings are configured
        ConfigureEntityMappings();

        var client = new MongoClient(options.Value.ConnectionString);
        _database = client.GetDatabase(options.Value.DatabaseName);
    }

    // Static method to ensure class mapping is configured
    public static void EnsureClassMappingsConfigured()
    {
        ConfigureEntityMappings();
    }

    private static void ConfigureEntityMappings()
    {
        // Configure Product entity
        if (!BsonClassMap.IsClassMapRegistered(typeof(Product)))
        {
            BsonClassMap.RegisterClassMap<Product>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(p => p.Id);
                cm.IdMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance);
            });
        }

        // Configure RequestResponseLog entity with explicit field mapping
        if (!BsonClassMap.IsClassMapRegistered(typeof(RequestResponseLog)))
        {
            BsonClassMap.RegisterClassMap<RequestResponseLog>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(p => p.Id);
                cm.IdMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance);
                
                // Explicitly map all properties to ensure consistency with existing data
                cm.MapProperty(p => p.TimestampUtc).SetElementName("timestampUtc");
                cm.MapProperty(p => p.TraceId).SetElementName("traceId");
                cm.MapProperty(p => p.Method).SetElementName("method");
                cm.MapProperty(p => p.Path).SetElementName("path");
                cm.MapProperty(p => p.StatusCode).SetElementName("statusCode");
                cm.MapProperty(p => p.RequestBody).SetElementName("requestBody");
                cm.MapProperty(p => p.ResponseBody).SetElementName("responseBody");
                cm.MapProperty(p => p.RequestHeaders).SetElementName("requestHeaders");
                cm.MapProperty(p => p.ResponseHeaders).SetElementName("responseHeaders");
            });
            
            Console.WriteLine("RequestResponseLog class mapping configured successfully");
        }
        else
        {
            Console.WriteLine("RequestResponseLog class mapping already registered");
        }
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);
}


