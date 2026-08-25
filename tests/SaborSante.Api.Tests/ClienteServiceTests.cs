public class ClienteServiceTests
{
    [Fact]
    public async Task CriarAsync_deve_falhar_quando_nome_estiver_vazio()
    {
        var repository = new FakeClienteRepository();
        var service = new ClienteService(repository);

        var request = new CriarClienteRequest(
            "",
            "47999999999"
        );

        var resultado = await service.CriarAsync(request);

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Validacao, resultado.TipoErro);
        Assert.Equal(
            "Nome é obrigatório.",
            resultado.Erro
        );
        Assert.Equal(
            0,
            repository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task CriarAsync_deve_retornar_sucesso_quando_dados_forems_validos()
    {
        var clienteCriado = new Cliente(
            1,
            "Thiago",
            "47999999999"
        );

        var repository = new FakeClienteRepository
        {
            ClienteParaRetornar = clienteCriado
        };

        var service = new ClienteService(repository);

        var request = new CriarClienteRequest(
            "Thiago",
            "47999999999"
        );

        var resultado = await service.CriarAsync(request);

        Assert.True(resultado.Sucesso);
        Assert.Equal(clienteCriado, resultado.Valor);
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
        Assert.Equal(
            1,
            repository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task CriarAsync_deve_retornar_conflito_quando_telefone_ja_existir()
    {
        var repository = new FakeClienteRepository()
        {
            ClienteParaRetornar = null
        };

        var service = new ClienteService(repository);

        var request = new CriarClienteRequest(
            "Thiago",
            "47999999999"
        );

        var resultado = await service.CriarAsync(request);

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Conflito, resultado.TipoErro);
        Assert.Equal(
            "Já existe um cliente com este telefone.",
            resultado.Erro
        );
        Assert.Null(resultado.Valor);
    }

    [Fact]
    public async Task CriarAsync_deve_normalizar_nome_e_telefone_antes_de_chamar_repository()
    {
        var clienteCriado = new Cliente(
            1,
            "Thiago",
            "47999999999"
        );

        var repository = new FakeClienteRepository()
        {
            ClienteParaRetornar = clienteCriado
        };

        var service = new ClienteService(repository);

        var request = new CriarClienteRequest(
            "  Thiago  ",
            "(47) 99999-9999"
        );

        await service.CriarAsync(request);

        Assert.Equal(
            "Thiago",
            repository.NomeRecebido
        );

        Assert.Equal(
            "47999999999",
            repository.TelefoneRecebido
        );

        Assert.Equal(
            1,
            repository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task CriarAsync_deve_falhar_quando_telefone_estiver_vazio()
    {
        var repository = new FakeClienteRepository();

        var service = new ClienteService(repository);

        var request = new CriarClienteRequest(
            "Thiago",
            ""
        );

        var resultado = await service.CriarAsync(request);

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Validacao, resultado.TipoErro);
        Assert.Equal("Telefone é obrigatório.", resultado.Erro);
    }

    [Fact]
    public async Task CriarAsync_deve_falhar_quando_telefone_nao_tiver_digitos()
    {
        var repository = new FakeClienteRepository();

        var service = new ClienteService(repository);

        var request = new CriarClienteRequest(
            "Thiago",
            "( ) -"
        );

        var resultado = await service.CriarAsync(request);

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Validacao, resultado.TipoErro);
        Assert.Equal("Telefone é obrigatório.", resultado.Erro);
    }

    [Fact]
    public async Task CriarAsync_deve_falhar_quando_nome_for_null()
    {
        var repository = new FakeClienteRepository();

        var service = new ClienteService(repository);

        var request = new CriarClienteRequest(
            null,
            "47999999999"
        );

        var resultado = await service.CriarAsync(request);

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Validacao, resultado.TipoErro);
        Assert.Equal("Nome é obrigatório.", resultado.Erro);
    }

    [Fact]
    public async Task CriarAsync_deve_falhar_quando_telefone_for_null()
    {
        var repository = new FakeClienteRepository();

        var service = new ClienteService(repository);

        var request = new CriarClienteRequest(
            "Thiago",
            null
        );

        var resultado = await service.CriarAsync(request);

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Validacao, resultado.TipoErro);
        Assert.Equal("Telefone é obrigatório.", resultado.Erro);
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_false_quando_cliente_nao_for_encontrado()
    {
        var repository = new FakeClienteRepository
        {
            ResultadoAtualizacaoParaRetornar =
                ResultadoAtualizacaoCliente.NaoEncontrado
        };

        var service = new ClienteService(repository);

        var request = new AtualizarClienteRequest(
            "Thiago",
            "47999999999"
        );

        var resultado =
            await service.AtualizarAsync(
                999,
                request
            );

        Assert.True(resultado.Sucesso);
        Assert.False(resultado.Valor);
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_conflito_quando_telefone_ja_existir()
    {
        var repository = new FakeClienteRepository
        {
            ResultadoAtualizacaoParaRetornar =
                ResultadoAtualizacaoCliente.Conflito
        };

        var service = new ClienteService(repository);

        var request = new AtualizarClienteRequest(
            "Thiago",
            "47999999999"
        );

        var resultado =
            await service.AtualizarAsync(
                1,
                request
            );

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Conflito, resultado.TipoErro);
        Assert.Equal(
            "Já existe um cliente com este telefone.",
            resultado.Erro
        );
        Assert.False(resultado.Valor);
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_sucesso_quando_cliente_for_atualizado()
    {
        var repository = new FakeClienteRepository
        {
            ResultadoAtualizacaoParaRetornar =
                ResultadoAtualizacaoCliente.Atualizado
        };

        var service = new ClienteService(repository);

        var request = new AtualizarClienteRequest(
            "Thiago",
            "47999999999"
        );

        var resultado =
            await service.AtualizarAsync(
                1,
                request
            );

        Assert.True(resultado.Sucesso);
        Assert.True(resultado.Valor);
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
        Assert.Equal(
            1,
            repository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task AtualizarAsync_deve_normalizar_dados_antes_de_chamar_repository()
    {
        var repository = new FakeClienteRepository
        {
            ResultadoAtualizacaoParaRetornar =
                ResultadoAtualizacaoCliente.Atualizado
        };

        var service = new ClienteService(repository);

        var request = new AtualizarClienteRequest(
            "  Thiago  ",
            "(47) 99999-9999"
        );

        await service.AtualizarAsync(
            10,
            request
        );

        Assert.Equal(
            10,
            repository.IdRecebidoAtualizar
        );

        Assert.Equal(
            "Thiago",
            repository.NomeRecebidoAtualizar
        );

        Assert.Equal(
            "47999999999",
            repository.TelefoneRecebidoAtualizar
        );
    }

    [Fact]
    public async Task AtualizarAsync_nao_deve_chamar_repository_quando_nome_estiver_vazio()
    {
        var repository = new FakeClienteRepository();

        var service = new ClienteService(repository);

        var request = new AtualizarClienteRequest(
            "",
            "47999999999"
        );

        var resultado = await service.AtualizarAsync(
            1,
            request
        );

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Validacao, resultado.TipoErro);
        Assert.Equal(
            "Nome é obrigatório.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            repository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_sucesso_quando_cliente_for_reativado()
    {
        var repository = new FakeClienteRepository
        {
            ResultadoReativacaoParaRetornar =
                ResultadoReativacaoCliente.Reativado
        };

        var service = new ClienteService(repository);

        var resultado = await service.ReativarAsync(1);

        Assert.True(resultado.Sucesso);
        Assert.True(resultado.Valor);
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
        Assert.Equal(
            1,
            repository.QuantidadeChamadasReativarAsync
        );
        Assert.Equal(
            1,
            repository.IdRecebidoReativar
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_false_quando_cliente_nao_for_encontrado()
    {
        var repository = new FakeClienteRepository
        {
            ResultadoReativacaoParaRetornar =
                ResultadoReativacaoCliente.NaoEncontrado
        };

        var service = new ClienteService(repository);

        var resultado = await service.ReativarAsync(999);

        Assert.True(resultado.Sucesso);
        Assert.False(resultado.Valor);
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_conflito_quando_cliente_ja_estiver_ativo()
    {
        var repository = new FakeClienteRepository
        {
            ResultadoReativacaoParaRetornar =
                ResultadoReativacaoCliente.JaAtivo
        };

        var service = new ClienteService(repository);

        var resultado = await service.ReativarAsync(1);

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Conflito, resultado.TipoErro);
        Assert.Equal(
            "O cliente já está ativo.",
            resultado.Erro
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_conflito_quando_telefone_estiver_em_uso()
    {
        var repository = new FakeClienteRepository
        {
            ResultadoReativacaoParaRetornar =
                ResultadoReativacaoCliente.Conflito
        };

        var service = new ClienteService(repository);

        var resultado = await service.ReativarAsync(1);

        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Conflito, resultado.TipoErro);
    }
}
