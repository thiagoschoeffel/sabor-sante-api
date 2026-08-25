using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SaborSante.Api.Tests;

public class PostgresFixture : IAsyncLifetime
{
    public NpgsqlDataSource DataSource { get; private set; } = null!;

    public Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.IntegrationTests.json")
            .Build();

        var connectionString =
            configuration.GetConnectionString("Postgres");

        DataSource =
            NpgsqlDataSource.Create(connectionString!);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await DataSource.DisposeAsync();
    }

    public ClienteRepository CriarClienteRepository()
    {
        return new ClienteRepository(DataSource);
    }

    public async Task LimparBancoAsync()
    {
        await using var connection =
            await DataSource.OpenConnectionAsync();

        await using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            DELETE FROM clientes_enderecos;
            DELETE FROM clientes;
            """;

        await command.ExecuteNonQueryAsync();
    }
}
