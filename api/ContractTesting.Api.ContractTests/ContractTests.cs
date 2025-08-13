using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ContractTesting.Api.ContractTests;

public class SomethingApiTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture fixture;
    private readonly ITestOutputHelper output;

    public SomethingApiTests(ApiFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.output = output;
    }

    [Fact]
    public void EnsureSomethingApiHonoursPactWithConsumer()
    {
        var config = new PactVerifierConfig
        {
            Outputters =
            [
                new XunitOutput(output),
            ],
        };

        using var pactVerifier = new PactVerifier("ContractTesting.Api", config);

        pactVerifier
            .WithHttpEndpoint(fixture.ServerUri)
            .WithFileSource(new FileInfo("../../../../../app/pacts/contract-testing-app-ContractTesting.Api.json"))
            .WithProviderStateUrl(new Uri(fixture.ServerUri, "/provider-states"))
            .Verify();
    }
}