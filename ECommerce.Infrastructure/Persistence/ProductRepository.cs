using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public sealed class ProductRepository(IMongoDbContext context) : IProductRepository
{
    private readonly IMongoCollection<Product> _collection = context.GetCollection<Product>("products");

    public async Task<Product> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        var result = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await result.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        var results = await _collection.FindAsync(Builders<Product>.Filter.Empty, cancellationToken: cancellationToken);
        return await results.ToListAsync(cancellationToken);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(product, cancellationToken: cancellationToken);
        return product;
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
        var options = new ReplaceOptions { IsUpsert = false };
        await _collection.ReplaceOneAsync(filter, product, options, cancellationToken);
        return product;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }
}


