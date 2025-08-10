using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ECommerce.Blazor.Services;

public interface ILogService
{
    Task<List<RequestResponseLog>> GetLogsAsync(LogFilter filter, CancellationToken cancellationToken = default);
    Task<long> GetLogsCountAsync(LogFilter filter, CancellationToken cancellationToken = default);
    Task<List<BsonDocument>> GetRawLogsAsync(int limit = 5, CancellationToken cancellationToken = default);
}

public class LogFilter
{
    public string? Method { get; set; }
    public string? Path { get; set; }
    public int? StatusCode { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? TraceId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class LogService : ILogService
{
    private readonly IMongoCollection<RequestResponseLog> _collection;

    public LogService(IMongoDbContext mongoDbContext)
    {
        _collection = mongoDbContext.GetCollection<RequestResponseLog>("http_logs");
    }

    public async Task<List<RequestResponseLog>> GetLogsAsync(LogFilter filter, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterBuilder = BuildFilter(filter);
            var sort = Builders<RequestResponseLog>.Sort.Descending(x => x.TimestampUtc);
            
            var skip = (filter.Page - 1) * filter.PageSize;
            
            return await _collection
                .Find(filterBuilder)
                .Sort(sort)
                .Skip(skip)
                .Limit(filter.PageSize)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"Error in GetLogsAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<long> GetLogsCountAsync(LogFilter filter, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterBuilder = BuildFilter(filter);
            return await _collection.CountDocumentsAsync(filterBuilder, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"Error in GetLogsCountAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<List<BsonDocument>> GetRawLogsAsync(int limit = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            var collection = _collection.Database.GetCollection<BsonDocument>("http_logs");
            return await collection.Find(new BsonDocument()).Limit(limit).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRawLogsAsync: {ex.Message}");
            throw;
        }
    }

    private FilterDefinition<RequestResponseLog> BuildFilter(LogFilter filter)
    {
        var builder = Builders<RequestResponseLog>.Filter;
        var filters = new List<FilterDefinition<RequestResponseLog>>();

        if (!string.IsNullOrWhiteSpace(filter.Method))
        {
            filters.Add(builder.Regex(x => x.Method, new MongoDB.Bson.BsonRegularExpression(filter.Method, "i")));
        }

        if (!string.IsNullOrWhiteSpace(filter.Path))
        {
            filters.Add(builder.Regex(x => x.Path, new MongoDB.Bson.BsonRegularExpression(filter.Path, "i")));
        }

        if (filter.StatusCode.HasValue)
        {
            filters.Add(builder.Eq(x => x.StatusCode, filter.StatusCode.Value));
        }

        if (filter.FromDate.HasValue)
        {
            filters.Add(builder.Gte(x => x.TimestampUtc, filter.FromDate.Value));
        }

        if (filter.ToDate.HasValue)
        {
            filters.Add(builder.Lte(x => x.TimestampUtc, filter.ToDate.Value));
        }

        if (!string.IsNullOrWhiteSpace(filter.TraceId))
        {
            filters.Add(builder.Regex(x => x.TraceId, new MongoDB.Bson.BsonRegularExpression(filter.TraceId, "i")));
        }

        return filters.Count > 0 ? builder.And(filters) : builder.Empty;
    }
}
