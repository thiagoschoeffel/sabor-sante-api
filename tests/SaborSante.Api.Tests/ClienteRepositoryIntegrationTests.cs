using Npgsql;

namespace SaborSante.Api.Tests;

[Collection("Postgres Integration")]
public class ClienteRepositoryIntegrationTests
    : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public ClienteRepositoryIntegrationTests(
    PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CriarAsync_deve_persistir_cliente_no_postgresql()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefone =
            DateTime.UtcNow.Ticks.ToString();

        var cliente = await repository.CriarAsync(
            "Cliente Integração",
            telefone
        );

        Assert.NotNull(cliente);
        Assert.True(cliente.Id > 0);
        Assert.Equal("Cliente Integração", cliente.Nome);
        Assert.Equal(telefone, cliente.Telefone);
    }

    [Fact]
    public async Task ObterPorIdAsync_deve_retornar_cliente_existente()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefone =
            DateTime.UtcNow.Ticks.ToString();

        var clienteCriado = await repository.CriarAsync(
            "Cliente Busca",
            telefone
        );

        Assert.NotNull(clienteCriado);

        var clienteObtido =
            await repository.ObterPorIdAsync(clienteCriado.Id);

        Assert.NotNull(clienteObtido);

        Assert.Equal(
            clienteCriado.Id,
            clienteObtido.Id
        );

        Assert.Equal(
            clienteCriado.Nome,
            clienteObtido.Nome
        );

        Assert.Equal(
            clienteCriado.Telefone,
            clienteObtido.Telefone
        );
    }

    [Fact]
    public async Task ObterPorIdAsync_deve_retornar_null_quando_cliente_nao_existir()
    {
        var repository =
            _fixture.CriarClienteRepository();

        var cliente =
            await repository.ObterPorIdAsync(int.MaxValue);

        Assert.Null(cliente);
    }

    [Fact]
    public async Task AtualizarAsync_deve_atualizar_cliente_existente()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefoneOriginal = DateTime.UtcNow.Ticks.ToString();
        var telefoneAtualizado = (DateTime.UtcNow.Ticks + 1).ToString();

        var clienteCriado = await repository.CriarAsync(
                        "Cliente Original",
                        telefoneOriginal
                    );

        Assert.NotNull(clienteCriado);

        var resultado = await repository.AtualizarAsync(
            clienteCriado.Id,
            "Cliente Atualizado",
            telefoneAtualizado
        );

        Assert.Equal(
            ResultadoAtualizacaoCliente.Atualizado,
            resultado
        );

        var clienteAtualizado =
            await repository.ObterPorIdAsync(clienteCriado.Id);

        Assert.NotNull(clienteAtualizado);
        Assert.Equal("Cliente Atualizado", clienteAtualizado.Nome);
        Assert.Equal(telefoneAtualizado, clienteAtualizado.Telefone);
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_nao_encontrado_quando_cliente_nao_existir()
    {
        var repository =
            _fixture.CriarClienteRepository();

        var resultado = await repository.AtualizarAsync(
            int.MaxValue,
            "Cliente Inexistente",
            "47999999999"
        );

        Assert.Equal(
            ResultadoAtualizacaoCliente.NaoEncontrado,
            resultado
        );
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_conflito_quando_telefone_ja_estiver_em_uso()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefone1 = DateTime.UtcNow.Ticks.ToString();
        var telefone2 = (DateTime.UtcNow.Ticks + 1).ToString();

        var cliente1 = await repository.CriarAsync(
            "Cliente 1",
            telefone1
        );

        var cliente2 = await repository.CriarAsync(
            "Cliente 2",
            telefone2
        );

        Assert.NotNull(cliente1);
        Assert.NotNull(cliente2);

        var resultado = await repository.AtualizarAsync(
            cliente2.Id,
            "Cliente 2",
            telefone1
        );

        Assert.Equal(
            ResultadoAtualizacaoCliente.Conflito,
            resultado
        );
    }

    [Fact]
    public async Task ExcluirAsync_deve_inativar_cliente_existente()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefone = DateTime.UtcNow.Ticks.ToString();

        var clienteCriado = await repository.CriarAsync(
            "Cliente Exclusão",
            telefone
        );

        Assert.NotNull(clienteCriado);

        var excluido =
            await repository.ExcluirAsync(clienteCriado.Id);

        Assert.True(excluido);

        var clienteObtido =
            await repository.ObterPorIdAsync(clienteCriado.Id);

        Assert.Null(clienteObtido);
    }

    [Fact]
    public async Task ExcluirAsync_deve_retornar_false_quando_cliente_nao_existir()
    {
        var repository =
            _fixture.CriarClienteRepository();

        var excluido =
            await repository.ExcluirAsync(int.MaxValue);

        Assert.False(excluido);
    }

    [Fact]
    public async Task ReativarAsync_deve_reativar_cliente_inativo()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefone = DateTime.UtcNow.Ticks.ToString();

        var clienteCriado = await repository.CriarAsync(
            "Cliente Reativação",
            telefone
        );

        Assert.NotNull(clienteCriado);

        var excluido =
            await repository.ExcluirAsync(clienteCriado.Id);

        Assert.True(excluido);

        var resultado =
            await repository.ReativarAsync(clienteCriado.Id);

        Assert.Equal(
            ResultadoReativacaoCliente.Reativado,
            resultado
        );

        var clienteObtido =
            await repository.ObterPorIdAsync(clienteCriado.Id);

        Assert.NotNull(clienteObtido);
        Assert.Equal(clienteCriado.Id, clienteObtido.Id);
        Assert.Equal(clienteCriado.Nome, clienteObtido.Nome);
        Assert.Equal(clienteCriado.Telefone, clienteObtido.Telefone);
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_nao_encontrado_quando_cliente_nao_existir()
    {
        var repository =
            _fixture.CriarClienteRepository();

        var resultado =
            await repository.ReativarAsync(int.MaxValue);

        Assert.Equal(
            ResultadoReativacaoCliente.NaoEncontrado,
            resultado
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_ja_ativo_quando_cliente_estiver_ativo()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefone = DateTime.UtcNow.Ticks.ToString();

        var clienteCriado = await repository.CriarAsync(
                        "Cliente Já Ativo",
                        telefone
                    );

        Assert.NotNull(clienteCriado);

        var resultado =
            await repository.ReativarAsync(clienteCriado.Id);

        Assert.Equal(
            ResultadoReativacaoCliente.JaAtivo,
            resultado
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_conflito_quando_telefone_estiver_em_uso_por_outro_cliente_ativo()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefone = DateTime.UtcNow.Ticks.ToString();

        var cliente1 = await repository.CriarAsync(
            "Cliente 1",
            telefone
        );

        Assert.NotNull(cliente1);

        var excluido =
            await repository.ExcluirAsync(cliente1.Id);

        Assert.True(excluido);

        var cliente2 = await repository.CriarAsync(
            "Cliente 2",
            telefone
        );

        Assert.NotNull(cliente2);

        var resultado =
            await repository.ReativarAsync(cliente1.Id);

        Assert.Equal(
            ResultadoReativacaoCliente.Conflito,
            resultado
        );
    }

    [Fact]
    public async Task CriarAsync_deve_retornar_null_quando_telefone_ja_estiver_em_uso()
    {
        await _fixture.LimparBancoAsync();

        var repository =
            _fixture.CriarClienteRepository();

        var telefone =
            DateTime.UtcNow.Ticks.ToString();

        var clienteExistente = await repository.CriarAsync(
            "Cliente Existente",
            telefone
        );

        Assert.NotNull(clienteExistente);

        var clienteDuplicado = await repository.CriarAsync(
            "Cliente Duplicado",
            telefone
        );

        Assert.Null(clienteDuplicado);
    }
}
