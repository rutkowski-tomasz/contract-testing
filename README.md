# Development

```sh
cd app && npm run dev # start app
cd api && dotnet run --project ./ContractTesting.Api/ContractTesting.Api.csproj # start api
docker run -t -p 8080:8080 -v "$(pwd)/pacts/:/app/pacts" pactfoundation/pact-stub-server -p 8080 -d pacts # start stub
```