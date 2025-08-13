using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ContractTesting.Api.ContractTests;

public class ApiFixture : IDisposable
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
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        services.AddExceptionHandler<ExceptionHandler>();
        services.AddProblemDetails();
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
        app.MapPost("provider-states", ([FromBody] ProviderStateRequest request) => request.State);
    }

    public class ProviderStateRequest
    {
        public string State { get; set; } = string.Empty;
        public Dictionary<string, object>? Params { get; set; }
    }

    public void Dispose() => app.DisposeAsync().AsTask().Wait();
}