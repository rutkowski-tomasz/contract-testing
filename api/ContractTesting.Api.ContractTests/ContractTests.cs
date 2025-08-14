using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ContractTesting.Api.ContractTests;

public class ContractTests(
    ApiFixture fixture,
    ITestOutputHelper output
) : IClassFixture<ApiFixture>
{
    [Fact]
    public void EnsureApiHonoursPactWithConsumer()
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