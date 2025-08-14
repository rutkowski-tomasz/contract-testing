using Bogus;

namespace ContractTesting.Api;

public static class Endpoints
{
    public class Product
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required decimal Price { get; init; }
    }

    public static void MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/health", () => true);

        builder.MapGet("/api/products", () =>
        {
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.IndexFaker + 1)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, (f, p) => $"Description for {p.Name}")
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(1, 100)));

            return productFaker.Generate(5).ToArray();
        });
    }
}