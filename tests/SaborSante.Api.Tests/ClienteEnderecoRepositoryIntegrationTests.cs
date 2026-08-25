using SaborSante.Api;

namespace SaborSante.Api.Tests;

[Collection("Postgres Integration")]
public class ClienteEnderecoRepositoryIntegrationTests
    : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public ClienteEnderecoRepositoryIntegrationTests(
        PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CriarAsync_deve_persistir_endereco_para_cliente_existente()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var telefone =
            DateTime.UtcNow.Ticks.ToString();

        var cliente = await clienteRepository.CriarAsync(
            "Cliente Endereço",
            telefone
        );

        Assert.NotNull(cliente);

        var endereco = await enderecoRepository.CriarAsync(
            cliente.Id,
            "Casa",
            "Rua das Flores",
            "123",
            "Apto 10",
            "Centro",
            "Blumenau",
            "89000000"
        );

        Assert.True(endereco.Id > 0);

        Assert.Equal(
            cliente.Id,
            endereco.ClienteId
        );

        Assert.Equal(
            "Casa",
            endereco.Identificacao
        );

        Assert.Equal(
            "Rua das Flores",
            endereco.Logradouro
        );

        Assert.Equal(
            "123",
            endereco.Numero
        );

        Assert.Equal(
            "Apto 10",
            endereco.Complemento
        );

        Assert.Equal(
            "Centro",
            endereco.Bairro
        );

        Assert.Equal(
            "Blumenau",
            endereco.Cidade
        );

        Assert.Equal(
            "89000000",
            endereco.Cep
        );

        Assert.True(endereco.Ativo);
    }

    [Fact]
    public async Task ObterPorIdAsync_deve_retornar_endereco_do_cliente()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var telefone =
            DateTime.UtcNow.Ticks.ToString();

        var cliente = await clienteRepository.CriarAsync(
            "Cliente Busca Endereço",
            telefone
        );

        Assert.NotNull(cliente);

        var enderecoCriado = await enderecoRepository.CriarAsync(
            cliente.Id,
            "Trabalho",
            "Rua XV de Novembro",
            "500",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var enderecoObtido =
            await enderecoRepository.ObterPorIdAsync(
                cliente.Id,
                enderecoCriado.Id
            );

        Assert.NotNull(enderecoObtido);

        Assert.Equal(
            enderecoCriado,
            enderecoObtido
        );
    }

    [Fact]
    public async Task ObterPorIdAsync_deve_retornar_null_quando_endereco_pertencer_a_outro_cliente()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente1 = await clienteRepository.CriarAsync(
            "Cliente 1",
            DateTime.UtcNow.Ticks.ToString()
        );

        var cliente2 = await clienteRepository.CriarAsync(
            "Cliente 2",
            (DateTime.UtcNow.Ticks + 1).ToString()
        );

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        var endereco = await enderecoRepository.CriarAsync(
            cliente1.Id,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var enderecoObtido =
            await enderecoRepository.ObterPorIdAsync(
                cliente2.Id,
                endereco.Id
            );

        Assert.Null(enderecoObtido);
    }

    [Fact]
    public async Task ListarPorClienteAsync_deve_retornar_apenas_enderecos_do_cliente()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente1 = await clienteRepository.CriarAsync(
            "Cliente 1",
            DateTime.UtcNow.Ticks.ToString()
        );

        var cliente2 = await clienteRepository.CriarAsync(
            "Cliente 2",
            (DateTime.UtcNow.Ticks + 1).ToString()
        );

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        var endereco1 = await enderecoRepository.CriarAsync(
            cliente1.Id,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var endereco2 = await enderecoRepository.CriarAsync(
            cliente1.Id,
            "Trabalho",
            "Rua B",
            "200",
            null,
            "Velha",
            "Blumenau",
            null
        );

        await enderecoRepository.CriarAsync(
            cliente2.Id,
            "Casa",
            "Rua C",
            "300",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var enderecos =
            await enderecoRepository.ListarPorClienteAsync(
                cliente1.Id
            );

        Assert.Equal(2, enderecos.Count);

        Assert.Contains(endereco1, enderecos);
        Assert.Contains(endereco2, enderecos);

        Assert.All(
            enderecos,
            endereco => Assert.Equal(
                cliente1.Id,
                endereco.ClienteId
            )
        );
    }

    [Fact]
    public async Task ListarPorClienteAsync_deve_ignorar_enderecos_inativos()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente = await clienteRepository.CriarAsync(
            "Cliente",
            DateTime.UtcNow.Ticks.ToString()
        );

        Assert.NotNull(cliente);

        var enderecoAtivo = await enderecoRepository.CriarAsync(
            cliente.Id,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var enderecoInativo = await enderecoRepository.CriarAsync(
            cliente.Id,
            "Trabalho",
            "Rua B",
            "200",
            null,
            "Velha",
            "Blumenau",
            null
        );

        var excluido = await enderecoRepository.ExcluirAsync(
            cliente.Id,
            enderecoInativo.Id
        );

        Assert.True(excluido);

        var enderecos =
            await enderecoRepository.ListarPorClienteAsync(
                cliente.Id
            );

        Assert.Single(enderecos);

        Assert.Contains(
            enderecoAtivo,
            enderecos
        );

        Assert.DoesNotContain(
            enderecos,
            endereco => endereco.Id == enderecoInativo.Id
        );
    }

    [Fact]
    public async Task AtualizarAsync_deve_atualizar_endereco_existente()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente = await clienteRepository.CriarAsync(
            "Cliente",
            DateTime.UtcNow.Ticks.ToString()
        );

        Assert.NotNull(cliente);

        var endereco = await enderecoRepository.CriarAsync(
            cliente.Id,
            "Casa",
            "Rua Antiga",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var atualizado = await enderecoRepository.AtualizarAsync(
            cliente.Id,
            endereco.Id,
            "Trabalho",
            "Rua Nova",
            "200",
            "Sala 5",
            "Velha",
            "Blumenau",
            "89000000"
        );

        Assert.True(atualizado);

        var enderecoAtualizado =
            await enderecoRepository.ObterPorIdAsync(
                cliente.Id,
                endereco.Id
            );

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
    public async Task AtualizarAsync_deve_retornar_false_quando_endereco_nao_existir()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente = await clienteRepository.CriarAsync(
            "Cliente",
            DateTime.UtcNow.Ticks.ToString()
        );

        Assert.NotNull(cliente);

        var atualizado = await enderecoRepository.AtualizarAsync(
            cliente.Id,
            int.MaxValue,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        Assert.False(atualizado);
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_false_quando_endereco_pertencer_a_outro_cliente()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente1 = await clienteRepository.CriarAsync(
            "Cliente 1",
            DateTime.UtcNow.Ticks.ToString()
        );

        var cliente2 = await clienteRepository.CriarAsync(
            "Cliente 2",
            (DateTime.UtcNow.Ticks + 1).ToString()
        );

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        var endereco = await enderecoRepository.CriarAsync(
            cliente1.Id,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var atualizado = await enderecoRepository.AtualizarAsync(
            cliente2.Id,
            endereco.Id,
            "Trabalho",
            "Rua B",
            "200",
            null,
            "Velha",
            "Blumenau",
            null
        );

        Assert.False(atualizado);
    }

    [Fact]
    public async Task ExcluirAsync_deve_inativar_endereco_existente()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente = await clienteRepository.CriarAsync(
            "Cliente",
            DateTime.UtcNow.Ticks.ToString()
        );

        Assert.NotNull(cliente);

        var endereco = await enderecoRepository.CriarAsync(
            cliente.Id,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var excluido = await enderecoRepository.ExcluirAsync(
            cliente.Id,
            endereco.Id
        );

        Assert.True(excluido);

        var enderecoObtido =
            await enderecoRepository.ObterPorIdAsync(
                cliente.Id,
                endereco.Id
            );

        Assert.Null(enderecoObtido);
    }

    [Fact]
    public async Task ExcluirAsync_deve_retornar_false_quando_endereco_nao_existir()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente = await clienteRepository.CriarAsync(
            "Cliente",
            DateTime.UtcNow.Ticks.ToString()
        );

        Assert.NotNull(cliente);

        var excluido = await enderecoRepository.ExcluirAsync(
            cliente.Id,
            int.MaxValue
        );

        Assert.False(excluido);
    }

    [Fact]
    public async Task ExcluirAsync_deve_retornar_false_quando_endereco_pertencer_a_outro_cliente()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente1 = await clienteRepository.CriarAsync(
            "Cliente 1",
            DateTime.UtcNow.Ticks.ToString()
        );

        var cliente2 = await clienteRepository.CriarAsync(
            "Cliente 2",
            (DateTime.UtcNow.Ticks + 1).ToString()
        );

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        var endereco = await enderecoRepository.CriarAsync(
            cliente1.Id,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var excluido = await enderecoRepository.ExcluirAsync(
            cliente2.Id,
            endereco.Id
        );

        Assert.False(excluido);
    }

    [Fact]
    public async Task ReativarAsync_deve_reativar_endereco_inativo()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente = await clienteRepository.CriarAsync(
            "Cliente",
            DateTime.UtcNow.Ticks.ToString()
        );

        Assert.NotNull(cliente);

        var endereco = await enderecoRepository.CriarAsync(
            cliente.Id,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var excluido = await enderecoRepository.ExcluirAsync(
            cliente.Id,
            endereco.Id
        );

        Assert.True(excluido);

        var resultado = await enderecoRepository.ReativarAsync(
            cliente.Id,
            endereco.Id
        );

        Assert.Equal(
            ResultadoReativacaoEndereco.Reativado,
            resultado
        );

        var enderecoObtido =
            await enderecoRepository.ObterPorIdAsync(
                cliente.Id,
                endereco.Id
            );

        Assert.NotNull(enderecoObtido);
        Assert.Equal(endereco.Id, enderecoObtido.Id);
        Assert.Equal(cliente.Id, enderecoObtido.ClienteId);
        Assert.True(enderecoObtido.Ativo);
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_nao_encontrado_quando_endereco_nao_existir()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente = await clienteRepository.CriarAsync(
            "Cliente",
            DateTime.UtcNow.Ticks.ToString()
        );

        Assert.NotNull(cliente);

        var resultado = await enderecoRepository.ReativarAsync(
            cliente.Id,
            int.MaxValue
        );

        Assert.Equal(
            ResultadoReativacaoEndereco.NaoEncontrado,
            resultado
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_ja_ativo_quando_endereco_estiver_ativo()
    {
        await _fixture.LimparBancoAsync();

        var clienteRepository =
            _fixture.CriarClienteRepository();

        var enderecoRepository =
            new ClienteEnderecoRepository(
                _fixture.DataSource
            );

        var cliente = await clienteRepository.CriarAsync(
            "Cliente",
            DateTime.UtcNow.Ticks.ToString()
        );

        Assert.NotNull(cliente);

        var endereco = await enderecoRepository.CriarAsync(
            cliente.Id,
            "Casa",
            "Rua A",
            "100",
            null,
            "Centro",
            "Blumenau",
            null
        );

        var resultado = await enderecoRepository.ReativarAsync(
            cliente.Id,
            endereco.Id
        );

        Assert.Equal(
            ResultadoReativacaoEndereco.JaAtivo,
            resultado
        );
    }
}
