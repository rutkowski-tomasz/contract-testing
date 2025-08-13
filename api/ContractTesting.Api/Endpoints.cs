namespace ContractTesting.Api;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/health", () => true);
    }
}