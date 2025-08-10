using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Products.Commands;

public record CreateProductCommand(string Name, string Description, decimal Price, int StockQuantity) : IRequest<Product>;

internal sealed class CreateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductRepository _productRepository = productRepository;

    public Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };
        return _productRepository.CreateAsync(product, cancellationToken);
    }
}


