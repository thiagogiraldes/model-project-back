# Modelo Backend DDD Enterprise (.NET 8)

Template completo de backend em .NET 8 com:

- DDD (Domain / Application / Infrastructure / Api)
- Mensageria com Kafka e Solace (publicação e consumo)
- Estrutura de testes (xUnit, FluentAssertions, Moq)
- OpenTelemetry (OTLP exporter) integrado na API
- Repositório in-memory (sem banco de dados)

## Estrutura

- `src/Modelo.Domain` – Entidades, Eventos, Interfaces de domínio
- `src/Modelo.Application` – Commands, Handlers (MediatR), Interfaces de aplicação
- `src/Modelo.Infrastructure` – Repositórios in-memory, Kafka, Solace, IoC
- `src/Modelo.Api` – ASP.NET Core Web API, Program.cs, Controllers
- `tests/Modelo.Tests` – Testes unitários

## Como rodar

```bash
dotnet restore
dotnet build

# API
dotnet run --project src/Modelo.Api/Modelo.Api.csproj

# Testes
dotnet test
```

Configure Kafka e Solace no `appsettings.json` da API conforme seu ambiente.
