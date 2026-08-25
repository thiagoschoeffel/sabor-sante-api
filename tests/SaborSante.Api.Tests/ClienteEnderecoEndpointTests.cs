using System.Net;
using System.Net.Http.Json;
using SaborSante.Api;

namespace SaborSante.Api.Tests;

[Collection("Postgres Integration")]
public class ClienteEnderecoEndpointTests
    : IClassFixture<ApiWebApplicationFactory>,
      IClassFixture<PostgresFixture>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly PostgresFixture _postgresFixture;

    public ClienteEnderecoEndpointTests(
        ApiWebApplicationFactory factory,
        PostgresFixture postgresFixture)
    {
        _factory = factory;
        _postgresFixture = postgresFixture;
    }

    [Fact]
    public async Task PostEndereco_deve_criar_endereco_para_cliente_existente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Endereço HTTP",
                "(47) 97777-7777"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            "Apto 10",
            "Centro",
            "Blumenau",
            "89000000"
        );

        var response = await client.PostAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos",
            request
        );

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode
        );

        var endereco =
            await response.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(endereco);
        Assert.True(endereco.Id > 0);
        Assert.Equal(cliente.Id, endereco.ClienteId);
        Assert.Equal("Casa", endereco.Identificacao);
        Assert.Equal("Rua das Flores", endereco.Logradouro);
        Assert.Equal("123", endereco.Numero);
        Assert.Equal("Apto 10", endereco.Complemento);
        Assert.Equal("Centro", endereco.Bairro);
        Assert.Equal("Blumenau", endereco.Cidade);
        Assert.Equal("89000000", endereco.Cep);
        Assert.True(endereco.Ativo);
    }

    [Fact]
    public async Task PostEndereco_deve_retornar_not_found_quando_cliente_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var response = await client.PostAsJsonAsync(
            $"/clientes/{int.MaxValue}/enderecos",
            request
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PostEndereco_deve_retornar_bad_request_quando_identificacao_for_invalida()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Endereço Inválido",
                "(47) 98888-8888"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var request = new CriarClienteEnderecoRequest(
            "   ",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var response = await client.PostAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos",
            request
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetEnderecoPorId_deve_retornar_endereco_existente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Busca Endereço HTTP",
                "(47) 99911-2233"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var enderecoResponse = await client.PostAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua A",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            enderecoResponse.StatusCode
        );

        var enderecoCriado =
            await enderecoResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(enderecoCriado);

        var response = await client.GetAsync(
            $"/clientes/{cliente.Id}/enderecos/{enderecoCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var enderecoObtido =
            await response.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(enderecoObtido);
        Assert.Equal(enderecoCriado, enderecoObtido);
    }

    [Fact]
    public async Task GetEnderecoPorId_deve_retornar_not_found_quando_endereco_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Busca Endereço",
                "(47) 99922-3344"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var response = await client.GetAsync(
            $"/clientes/{cliente.Id}/enderecos/{int.MaxValue}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetEnderecoPorId_deve_retornar_not_found_quando_endereco_pertencer_a_outro_cliente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var cliente1Response = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 1",
                "(47) 91111-2222"
            )
        );

        var cliente2Response = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 2",
                "(47) 93333-4444"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            cliente1Response.StatusCode
        );

        Assert.Equal(
            HttpStatusCode.Created,
            cliente2Response.StatusCode
        );

        var cliente1 =
            await cliente1Response.Content.ReadFromJsonAsync<Cliente>();

        var cliente2 =
            await cliente2Response.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        var enderecoResponse = await client.PostAsJsonAsync(
            $"/clientes/{cliente1.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua A",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            enderecoResponse.StatusCode
        );

        var endereco =
            await enderecoResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(endereco);

        var response = await client.GetAsync(
            $"/clientes/{cliente2.Id}/enderecos/{endereco.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetEnderecos_deve_retornar_apenas_enderecos_do_cliente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var cliente1Response = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 1",
                "(47) 91111-3333"
            )
        );

        var cliente2Response = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 2",
                "(47) 92222-4444"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            cliente1Response.StatusCode
        );

        Assert.Equal(
            HttpStatusCode.Created,
            cliente2Response.StatusCode
        );

        var cliente1 =
            await cliente1Response.Content.ReadFromJsonAsync<Cliente>();

        var cliente2 =
            await cliente2Response.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        await client.PostAsJsonAsync(
            $"/clientes/{cliente1.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua A",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        await client.PostAsJsonAsync(
            $"/clientes/{cliente1.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Trabalho",
                "Rua B",
                "200",
                null,
                "Velha",
                "Blumenau",
                null
            )
        );

        await client.PostAsJsonAsync(
            $"/clientes/{cliente2.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua C",
                "300",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        var response = await client.GetAsync(
            $"/clientes/{cliente1.Id}/enderecos"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var enderecos =
            await response.Content.ReadFromJsonAsync<List<ClienteEndereco>>();

        Assert.NotNull(enderecos);
        Assert.Equal(2, enderecos.Count);

        Assert.All(
            enderecos,
            endereco => Assert.Equal(
                cliente1.Id,
                endereco.ClienteId
            )
        );
    }

    [Fact]
    public async Task PutEndereco_deve_atualizar_endereco_existente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Atualização Endereço",
                "(47) 94444-5555"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var enderecoResponse = await client.PostAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua Antiga",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            enderecoResponse.StatusCode
        );

        var endereco =
            await enderecoResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(endereco);

        var request = new AtualizarClienteEnderecoRequest(
            "Trabalho",
            "Rua Nova",
            "200",
            "Sala 5",
            "Velha",
            "Blumenau",
            "89000000"
        );

        var response = await client.PutAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos/{endereco.Id}",
            request
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );

        var getResponse = await client.GetAsync(
            $"/clientes/{cliente.Id}/enderecos/{endereco.Id}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode
        );

        var enderecoAtualizado =
            await getResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(enderecoAtualizado);
        Assert.Equal("Trabalho", enderecoAtualizado.Identificacao);
        Assert.Equal("Rua Nova", enderecoAtualizado.Logradouro);
        Assert.Equal("200", enderecoAtualizado.Numero);
        Assert.Equal("Sala 5", enderecoAtualizado.Complemento);
        Assert.Equal("Velha", enderecoAtualizado.Bairro);
        Assert.Equal("Blumenau", enderecoAtualizado.Cidade);
        Assert.Equal("89000000", enderecoAtualizado.Cep);
    }

    [Fact]
    public async Task PutEndereco_deve_retornar_not_found_quando_endereco_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Atualização Endereço",
                "(47) 95555-6666"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var response = await client.PutAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos/{int.MaxValue}",
            request
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PutEndereco_deve_retornar_not_found_quando_endereco_pertencer_a_outro_cliente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var cliente1Response = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 1",
                "(47) 96666-7777"
            )
        );

        var cliente2Response = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 2",
                "(47) 97777-8888"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            cliente1Response.StatusCode
        );

        Assert.Equal(
            HttpStatusCode.Created,
            cliente2Response.StatusCode
        );

        var cliente1 =
            await cliente1Response.Content.ReadFromJsonAsync<Cliente>();

        var cliente2 =
            await cliente2Response.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        var enderecoResponse = await client.PostAsJsonAsync(
            $"/clientes/{cliente1.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua A",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            enderecoResponse.StatusCode
        );

        var endereco =
            await enderecoResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(endereco);

        var request = new AtualizarClienteEnderecoRequest(
            "Trabalho",
            "Rua B",
            "200",
            null,
            "Velha",
            "Blumenau",
            null
        );

        var response = await client.PutAsJsonAsync(
            $"/clientes/{cliente2.Id}/enderecos/{endereco.Id}",
            request
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task DeleteEndereco_deve_inativar_endereco_existente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Exclusão Endereço",
                "(47) 98888-9999"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var enderecoResponse = await client.PostAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua A",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            enderecoResponse.StatusCode
        );

        var endereco =
            await enderecoResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(endereco);

        var response = await client.DeleteAsync(
            $"/clientes/{cliente.Id}/enderecos/{endereco.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );

        var getResponse = await client.GetAsync(
            $"/clientes/{cliente.Id}/enderecos/{endereco.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode
        );
    }

    [Fact]
    public async Task DeleteEndereco_deve_retornar_not_found_quando_endereco_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Exclusão Endereço",
                "(47) 99900-1111"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var response = await client.DeleteAsync(
            $"/clientes/{cliente.Id}/enderecos/{int.MaxValue}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task DeleteEndereco_deve_retornar_not_found_quando_endereco_pertencer_a_outro_cliente()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var cliente1Response = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 1",
                "(47) 91100-2200"
            )
        );

        var cliente2Response = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente 2",
                "(47) 92200-3300"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            cliente1Response.StatusCode
        );

        Assert.Equal(
            HttpStatusCode.Created,
            cliente2Response.StatusCode
        );

        var cliente1 =
            await cliente1Response.Content.ReadFromJsonAsync<Cliente>();

        var cliente2 =
            await cliente2Response.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        var enderecoResponse = await client.PostAsJsonAsync(
            $"/clientes/{cliente1.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua A",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            enderecoResponse.StatusCode
        );

        var endereco =
            await enderecoResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(endereco);

        var response = await client.DeleteAsync(
            $"/clientes/{cliente2.Id}/enderecos/{endereco.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PatchReativarEndereco_deve_reativar_endereco_inativo()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Reativação Endereço",
                "(47) 93300-4400"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var enderecoResponse = await client.PostAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua A",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            enderecoResponse.StatusCode
        );

        var endereco =
            await enderecoResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(endereco);

        var deleteResponse = await client.DeleteAsync(
            $"/clientes/{cliente.Id}/enderecos/{endereco.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode
        );

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/clientes/{cliente.Id}/enderecos/{endereco.Id}/reativar"
        );

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );

        var getResponse = await client.GetAsync(
            $"/clientes/{cliente.Id}/enderecos/{endereco.Id}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode
        );
    }

    [Fact]
    public async Task PatchReativarEndereco_deve_retornar_conflito_quando_endereco_ja_estiver_ativo()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Endereço Ativo",
                "(47) 94400-5500"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var enderecoResponse = await client.PostAsJsonAsync(
            $"/clientes/{cliente.Id}/enderecos",
            new CriarClienteEnderecoRequest(
                "Casa",
                "Rua A",
                "100",
                null,
                "Centro",
                "Blumenau",
                null
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            enderecoResponse.StatusCode
        );

        var endereco =
            await enderecoResponse.Content.ReadFromJsonAsync<ClienteEndereco>();

        Assert.NotNull(endereco);

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/clientes/{cliente.Id}/enderecos/{endereco.Id}/reativar"
        );

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PatchReativarEndereco_deve_retornar_not_found_quando_endereco_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var clienteResponse = await client.PostAsJsonAsync(
            "/clientes",
            new CriarClienteRequest(
                "Cliente Reativação Endereço",
                "(47) 95500-6600"
            )
        );

        Assert.Equal(
            HttpStatusCode.Created,
            clienteResponse.StatusCode
        );

        var cliente =
            await clienteResponse.Content.ReadFromJsonAsync<Cliente>();

        Assert.NotNull(cliente);

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/clientes/{cliente.Id}/enderecos/{int.MaxValue}/reativar"
        );

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task PatchReativarEndereco_deve_retornar_not_found_quando_cliente_nao_existir()
    {
        await _postgresFixture.LimparBancoAsync();

        var client = _factory.CreateClient();

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/clientes/{int.MaxValue}/enderecos/{int.MaxValue}/reativar"
        );

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }
}
