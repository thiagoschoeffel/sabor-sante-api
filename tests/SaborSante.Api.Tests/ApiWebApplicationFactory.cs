using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace SaborSante.Api.Tests;

public class ApiWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (_, config) =>
            {
                config.AddJsonFile(
                    "appsettings.IntegrationTests.json"
                );
            }
        );

        builder.ConfigureTestServices(
            services =>
            {
                services.RemoveAll<NpgsqlDataSource>();

                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(
                        "appsettings.IntegrationTests.json"
                    )
                    .Build();

                var connectionString =
                    configuration.GetConnectionString(
                        "Postgres"
                    );

                var dataSource =
                    NpgsqlDataSource.Create(
                        connectionString!
                    );

                services.AddSingleton(dataSource);
            }
        );
    }
}
