# 🤝 contract-testing

A **full-stack** sample project showcasing **consumer-driven contract testing** with **Pact** between a **React + TypeScript** frontend and a **.NET 9.0 API**.

## 🏗 Architecture
- **Frontend**: React + TypeScript + Vite  
- **Backend**: .NET 9 + Entity Framework Core + JWT Auth, simple CRUD (see [sample requests](https://github.com/rutkowski-tomasz/contract-testing/blob/main/requests.http))
- **Contracts**: Pact (consumer-driven), integrated in CI with automated verification

## ⚡ Quick Start

```sh
# Start API
cd api && dotnet run --project ./ContractTesting.Api/ContractTesting.Api.csproj

# Start Frontend
cd app && npm install && npm run dev

# (instead of API) Start Pact stub server – simulates API from contract
docker run -t -p 8080:8080 \
  -v "$(pwd)/pacts/:/app/pacts" \
  pactfoundation/pact-stub-server -p 8080 -d pacts
```

## 🔍 Contract Testing Flow
1. **Consumer tests** (frontend) generate [pact file](https://github.com/rutkowski-tomasz/contract-testing/blob/main/app/pacts/contract-testing-app-ContractTesting.Api.json) using `cd app && npm run test:pact`
2. **Provider verification** (backend) ensures API meets expectations using `cd api && dotnet test`
3. **CI** automatically runs verification to prevent breaking changes  

## 📂 Project Structure
```sh
contract-testing/
├── api/                           # .NET 9 API + provider tests
│   ├── ContractTesting.Api/
│   └── ContractTesting.Api.ContractTests/
├── app/                           # React app + consumer tests
│   ├── src/
│   ├── tests/
│   └── pacts/                     # Generated contract files
└── .github/workflows/ci.yml       # CI pipeline with contract verification
```
