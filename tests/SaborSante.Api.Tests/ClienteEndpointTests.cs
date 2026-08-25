using System.Net;
using System.Net.Http.Json;

namespace SaborSante.Api.Tests;

[Collection("Postgres Integration")]
public class ClienteEndpointTests
    : IClassFixture<ApiWebApplicationFactory>,
      IClassFixture<PostgresFixture>
{
    private readonly ApiWebApplicationFactory _factory;

    private readonly PostgresFixture _postgresFixture;

    public ClienteEndpointTests(
        ApiWebApplicationFactory factory,
        PostgresFixture postgresFixture)
    {
        _factory = factory;
        _postgresFixture = postgresFixture;
    }

    [Fact]
    public async Task PostClientes_deve_criar_cliente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var request = new CriarClienteRequest(
            "Cliente HTTP",
            "(47) 99999-9999"
        );

        var response = await client.PostAsJsonAsync(
            "/clientes",
            request
        );

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode
        );

        var cliente =
            await response.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        Assert.True(cliente.Id > 0);
        Assert.Equal("Cliente HTTP", cliente.Nome);
        Assert.Equal("47999999999", cliente.Telefone);
    }

    [Fact]
    public async Task PostClientes_deve_retornar_bad_request_quando_nome_for_invalido()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var request = new CriarClienteRequest(
            "   ",
            "(47) 99999-9999"
        );

        var response = await client.PostAsJsonAsync(
            "/clientes",
            request
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PostClientes_deve_retornar_conflito_quando_telefone_ja_estiver_em_uso()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var request = new CriarClienteRequest(
            "Cliente 1",
            "(47) 99999-9999"
        );

        var primeiraResposta = await client.PostAsJsonAsync(
            "/clientes",
            request
        );

        Assert.Equal(
            HttpStatusCode.Created,
            primeiraResposta.StatusCode
        );

        var requestDuplicado = new CriarClienteRequest(
            "Cliente 2",
            "(47) 99999-9999"
        );

        var response = await client.PostAsJsonAsync(
            "/clientes",
            requestDuplicado
        );

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetClientePorId_deve_retornar_cliente_existente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var request = new CriarClienteRequest(
            "Cliente Busca HTTP",
            "(47) 98888-7777"
        );

        var postResponse = await client.PostAsJsonAsync(
            "/clientes",
            request
        );

        Assert.Equal(
            HttpStatusCode.Created,
            postResponse.StatusCode
        );

        var clienteCriado =
            await postResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(clienteCriado);

        var response = await client.GetAsync(
            $"/clientes/{clienteCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var clienteObtido =
            await response.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(clienteObtido);
        Assert.Equal(clienteCriado, clienteObtido);
    }

    [Fact]
    public async Task GetClientePorId_deve_retornar_not_found_quando_cliente_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            $"/clientes/{int.MaxValue}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PutCliente_deve_atualizar_cliente_existente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var criarRequest = new CriarClienteRequest(
            "Cliente Original",
            "(47) 97777-6666"
        );

        var postResponse = await client.PostAsJsonAsync(
            "/clientes",
            criarRequest
        );

        Assert.Equal(
            HttpStatusCode.Created,
            postResponse.StatusCode
        );

        var clienteCriado =
            await postResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(clienteCriado);

        var atualizarRequest = new AtualizarClienteRequest(
            "Cliente Atualizado",
            "(47) 96666-5555"
        );

        var response = await client.PutAsJsonAsync(
            $"/clientes/{clienteCriado.Id}",
            atualizarRequest
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );

        var getResponse = await client.GetAsync(
            $"/clientes/{clienteCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode
        );

        var clienteAtualizado =
            await getResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(clienteAtualizado);
        Assert.Equal("Cliente Atualizado", clienteAtualizado.Nome);
        Assert.Equal("47966665555", clienteAtualizado.Telefone);
    }

    [Fact]
    public async Task PutCliente_deve_retornar_not_found_quando_cliente_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var request = new AtualizarClienteRequest(
            "Cliente Inexistente",
            "(47) 95555-4444"
        );

        var response = await client.PutAsJsonAsync(
            $"/clientes/{int.MaxValue}",
            request
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PutCliente_deve_retornar_conflito_quando_telefone_ja_estiver_em_uso()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var primeiroResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 1",
                "(47) 91111-1111"
            )
        );

        var segundoResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 2",
                "(47) 92222-2222"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            primeiroResponse.StatusCode
        );

        Assert.Equal(
            HttpStatusCode.Created,
            segundoResponse.StatusCode
        );

        var primeiroCliente =
            await primeiroResponse.Content.ReadFromJsonAsync<Cliente>();

        var segundoCliente =
            await segundoResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(primeiroCliente);
        Assert.NotNull(segundoCliente);

        var atualizarRequest = new AtualizarClienteRequest(
            "Cliente 2 Atualizado",
            primeiroCliente.Telefone
        );

        var response = await client.PutAsJsonAsync(
            $"/clientes/{segundoCliente.Id}",
            atualizarRequest
        );

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode
        );
    }

    [Fact]
    public async Task DeleteCliente_deve_inativar_cliente_existente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Exclusão HTTP",
                "(47) 93333-3333"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            postResponse.StatusCode
        );

        var clienteCriado =
            await postResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(clienteCriado);

        var response = await client.DeleteAsync(
            $"/clientes/{clienteCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );

        var getResponse = await client.GetAsync(
            $"/clientes/{clienteCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode
        );
    }

    [Fact]
    public async Task DeleteCliente_deve_retornar_not_found_quando_cliente_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var response = await client.DeleteAsync(
            $"/clientes/{int.MaxValue}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PatchReativarCliente_deve_reativar_cliente_inativo()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Reativação HTTP",
                "(47) 94444-4444"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            postResponse.StatusCode
        );

        var clienteCriado =
            await postResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(clienteCriado);

        var deleteResponse = await client.DeleteAsync(
            $"/clientes/{clienteCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode
        );

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/clientes/{clienteCriado.Id}/reativar"
        );

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );

        var getResponse = await client.GetAsync(
            $"/clientes/{clienteCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode
        );
    }

    [Fact]
    public async Task PatchReativarCliente_deve_retornar_conflito_quando_cliente_ja_estiver_ativo()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Ativo HTTP",
                "(47) 95555-5555"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            postResponse.StatusCode
        );

        var clienteCriado =
            await postResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(clienteCriado);

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/clientes/{clienteCriado.Id}/reativar"
        );

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PatchReativarCliente_deve_retornar_not_found_quando_cliente_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/clientes/{int.MaxValue}/reativar"
        );

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PatchReativarCliente_deve_retornar_conflito_quando_telefone_estiver_em_uso_por_outro_cliente_ativo()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var telefone = "(47) 96666-6666";

        var primeiroResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 1",
                telefone
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            primeiroResponse.StatusCode
        );

        var cliente1 =
            await primeiroResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente1);

        var deleteResponse = await client.DeleteAsync(
            $"/clientes/{cliente1.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode
        );

        var segundoResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 2",
                telefone
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            segundoResponse.StatusCode
        );

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/clientes/{cliente1.Id}/reativar"
        );

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode
        );
    }
}
