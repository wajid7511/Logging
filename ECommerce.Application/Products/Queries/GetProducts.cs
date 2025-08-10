using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Products.Queries;

public record GetProductsQuery() : IRequest<IReadOnlyList<Product>>;

internal sealed class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, IReadOnlyList<Product>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public Task<IReadOnlyList<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return _productRepository.GetAllAsync(cancellationToken);
    }
}


