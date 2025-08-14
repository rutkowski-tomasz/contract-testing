using Microsoft.EntityFrameworkCore;

namespace ContractTesting.Api;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/health", () => Results.Ok());

        builder.MapPost("/api/login", (LoginRequest request, IServiceProvider serviceProvider) =>
        {
            var token = AuthService.ValidateAndGenerateToken(request.Email, request.Password);
            return token is null
                ? Results.Unauthorized()
                : Results.Ok(new AuthResponse(token));
        });

        builder.MapGet("/api/products", async (ApplicationDbContext context) =>
        {
            return await context.Products.ToArrayAsync();
        }).RequireAuthorization();

        builder.MapGet("/api/products/{id}", async (int id, ApplicationDbContext context) =>
        {
            var product = await context.Products.FindAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        }).RequireAuthorization();

        builder.MapPost("/api/products", async (Product product, ApplicationDbContext context) =>
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return Results.Created($"/api/products/{product.Id}", product);
        }).RequireAuthorization();

        builder.MapPut("/api/products/{id}", async (int id, Product updatedProduct, ApplicationDbContext context) =>
        {
            var product = await context.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;

            await context.SaveChangesAsync();
            return Results.Ok(product);
        }).RequireAuthorization();

        builder.MapDelete("/api/products/{id}", async (int id, ApplicationDbContext context) =>
        {
            var product = await context.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}