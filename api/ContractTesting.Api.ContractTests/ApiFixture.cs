using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Bogus;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace ContractTesting.Api.ContractTests;

public partial class ApiFixture : IDisposable
{
    private readonly WebApplication app;
    public Uri ServerUri { get; }

    public ApiFixture()
    {
        ServerUri = new Uri("http://localhost:5280");
        app = CreateTestApp();
        app.StartAsync().Wait();
    }

    private WebApplication CreateTestApp()
    {
        var builder = WebApplication.CreateBuilder();
        
        ConfigureServices(builder.Services);
        builder.WebHost.UseUrls(ServerUri.ToString());
        
        var app = builder.Build();
        ConfigurePipeline(app);
        
        return app;
    }    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        services.AddExceptionHandler<ExceptionHandler>();
        services.AddProblemDetails();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("ContractTestingDb"));
            
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder => 
                policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });
    }
    
    private static void ConfigurePipeline(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseExceptionHandler();

        app.MapEndpoints();
        app.MapPost("provider-states", async ([FromBody] ProviderStateRequest request, ApplicationDbContext context) =>
        {
            if (request.State == "products exist")
            {
                await SeedProductsAsync(context);
            }

            var match = ProductWithIdExistsRegex().Match(request.State);
            if (match.Success)
            {
                int productId = int.Parse(match.Groups[1].Value);
                await SeedProductAsync(productId, context);
            }

            return Results.Ok();
        });
    }
    
    private static async Task SeedProductsAsync(ApplicationDbContext context)
    {
        var productFaker = new Faker<Product>()
            .CustomInstantiator(f => new Product
            {
                Name = f.Commerce.ProductName(),
                Description = f.Commerce.ProductDescription(),
                Price = f.Random.Decimal(1, 100)
            });
            
        var products = productFaker.Generate(5);
        
        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductAsync(int productId, ApplicationDbContext context)
    {
        var existingProduct = await context.Products.FindAsync(productId);
        if (existingProduct is not null)
        {
            return;
        }

        var productFaker = new Faker<Product>()
            .CustomInstantiator(f => new Product
            {
                Id = productId,
                Name = f.Commerce.ProductName(),
                Description = f.Commerce.ProductDescription(),
                Price = f.Random.Decimal(1, 100)
            });
            
        var product = productFaker.Generate();
        
        context.Products.Add(product);
        await context.SaveChangesAsync();
    }

    public class ProviderStateRequest
    {
        public string State { get; set; } = string.Empty;
        public Dictionary<string, object>? Params { get; set; }
    }

    public void Dispose()
    {
        app.DisposeAsync().AsTask().Wait();
        GC.SuppressFinalize(this);
    }

    [GeneratedRegex(@"product with id (\d+) exists")]
    private static partial Regex ProductWithIdExistsRegex();
}