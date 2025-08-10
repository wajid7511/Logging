using ECommerce.Domain.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public interface IRequestResponseLogRepository
{
    Task InsertAsync(RequestResponseLog log, CancellationToken cancellationToken);
}

public sealed class RequestResponseLogRepository(IMongoDbContext context, ILogger<RequestResponseLogRepository> logger) : IRequestResponseLogRepository
{
    private readonly IMongoCollection<RequestResponseLog> _collection = context.GetCollection<RequestResponseLog>("http_logs");
    private readonly ILogger<RequestResponseLogRepository> _logger = logger;

    public async Task InsertAsync(RequestResponseLog log, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to insert log for {Method} {Path} into MongoDB", log.Method, log.Path);
            
            await _collection.InsertOneAsync(log, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Successfully inserted log with ID {Id} into MongoDB collection {CollectionName}", 
                log.Id, "http_logs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert log into MongoDB for {Method} {Path}", log.Method, log.Path);
            throw;
        }
    }
}


