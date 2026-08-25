public class ClienteEnderecoServiceTests
{
    [Fact]
    public async Task CriarAsync_deve_retornar_nao_encontrado_quando_cliente_nao_existir()
    {
        var clienteRepository = new FakeClienteRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            null!
        );

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.CriarAsync(
            999,
            request
        );

        Assert.False(resultado.Sucesso);
        Assert.Equal(
            TipoErro.NaoEncontrado,
            resultado.TipoErro
        );
        Assert.Equal(
            "Cliente não encontrado.",
            resultado.Erro
        );
    }

    [Fact]
    public async Task CriarAsync_deve_tentar_criar_endereco_quando_cliente_existir_e_dados_forems_validos()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoCriado = new ClienteEndereco(
            1,
            1,
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000",
            true
        );

        var enderecoRepository = new FakeClienteEnderecoRepository
        {
            EnderecoParaRetornar = enderecoCriado
        };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.CriarAsync(
            1,
            request
        );

        Assert.True(resultado.Sucesso);
        Assert.Equal(
            enderecoCriado,
            resultado.Valor
        );
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
        Assert.Equal(
            1,
            enderecoRepository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task CriarAsync_deve_normalizar_dados_antes_de_chamar_repository()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoCriado = new ClienteEndereco(
            1,
            1,
            "Casa",
            "Rua das Flores",
            "123",
            "Apto 201",
            "Centro",
            "Blumenau",
            "89000-000",
            true
        );

        var enderecoRepository = new FakeClienteEnderecoRepository
        {
            EnderecoParaRetornar = enderecoCriado
        };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new CriarClienteEnderecoRequest(
            "  Casa  ",
            "  Rua das Flores  ",
            "  123  ",
            "  Apto 201  ",
            "  Centro  ",
            "  Blumenau  ",
            "  89000-000  "
        );

        await service.CriarAsync(
            1,
            request
        );

        Assert.Equal(
            1,
            enderecoRepository.ClienteIdRecebidoCriar
        );

        Assert.Equal(
            "Casa",
            enderecoRepository.IdentificacaoRecebidaCriar
        );

        Assert.Equal(
            "Rua das Flores",
            enderecoRepository.LogradouroRecebidoCriar
        );

        Assert.Equal(
            "123",
            enderecoRepository.NumeroRecebidoCriar
        );

        Assert.Equal(
            "Apto 201",
            enderecoRepository.ComplementoRecebidoCriar
        );

        Assert.Equal(
            "Centro",
            enderecoRepository.BairroRecebidoCriar
        );

        Assert.Equal(
            "Blumenau",
            enderecoRepository.CidadeRecebidaCriar
        );

        Assert.Equal(
            "89000-000",
            enderecoRepository.CepRecebidoCriar
        );
    }

    [Fact]
    public async Task CriarAsync_deve_converter_campos_opcionais_vazios_para_null()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoCriado = new ClienteEndereco(
            1,
            1,
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            null,
            true
        );

        var enderecoRepository = new FakeClienteEnderecoRepository
        {
            EnderecoParaRetornar = enderecoCriado
        };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            "   ",
            "Centro",
            "Blumenau",
            "   "
        );

        await service.CriarAsync(
            1,
            request
        );

        Assert.Null(
            enderecoRepository.ComplementoRecebidoCriar
        );

        Assert.Null(
            enderecoRepository.CepRecebidoCriar
        );
    }

    [Fact]
    public async Task CriarAsync_nao_deve_chamar_repository_quando_identificacao_estiver_vazia()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new CriarClienteEnderecoRequest(
            "",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.CriarAsync(
            1,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Identificação é obrigatória.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task CriarAsync_nao_deve_chamar_repository_quando_logradouro_estiver_vazio()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.CriarAsync(
            1,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Logradouro é obrigatório.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task CriarAsync_nao_deve_chamar_repository_quando_numero_estiver_vazio()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.CriarAsync(
            1,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Número é obrigatório.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task CriarAsync_nao_deve_chamar_repository_quando_bairro_estiver_vazio()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.CriarAsync(
            1,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Bairro é obrigatório.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task CriarAsync_nao_deve_chamar_repository_quando_cidade_estiver_vazia()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new CriarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "",
            "89000-000"
        );

        var resultado = await service.CriarAsync(
            1,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Cidade é obrigatória.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasCriarAsync
        );
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_sucesso_quando_endereco_for_atualizado()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoAtualizacaoParaRetornar = true
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.AtualizarAsync(
            1,
            10,
            request
        );

        Assert.True(resultado.Sucesso);
        Assert.True(resultado.Valor);
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
        Assert.Equal(
            1,
            enderecoRepository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_nao_encontrado_quando_endereco_nao_existir()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoAtualizacaoParaRetornar = false
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.AtualizarAsync(
            1,
            999,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.NaoEncontrado,
            resultado.TipoErro
        );

        Assert.Equal(
            "Endereço não encontrado.",
            resultado.Erro
        );
    }

    [Fact]
    public async Task AtualizarAsync_deve_retornar_nao_encontrado_quando_cliente_nao_existir()
    {
        var clienteRepository = new FakeClienteRepository();

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.AtualizarAsync(
            999,
            10,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.NaoEncontrado,
            resultado.TipoErro
        );

        Assert.Equal(
            "Cliente não encontrado.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task AtualizarAsync_deve_normalizar_dados_antes_de_chamar_repository()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoAtualizacaoParaRetornar = true
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "  Casa  ",
            "  Rua das Flores  ",
            "  123  ",
            "  Apto 201  ",
            "  Centro  ",
            "  Blumenau  ",
            "  89000-000  "
        );

        await service.AtualizarAsync(
            1,
            10,
            request
        );

        Assert.Equal(
            1,
            enderecoRepository.ClienteIdRecebidoAtualizar
        );

        Assert.Equal(
            10,
            enderecoRepository.EnderecoIdRecebidoAtualizar
        );

        Assert.Equal(
            "Casa",
            enderecoRepository.IdentificacaoRecebidaAtualizar
        );

        Assert.Equal(
            "Rua das Flores",
            enderecoRepository.LogradouroRecebidoAtualizar
        );

        Assert.Equal(
            "123",
            enderecoRepository.NumeroRecebidoAtualizar
        );

        Assert.Equal(
            "Apto 201",
            enderecoRepository.ComplementoRecebidoAtualizar
        );

        Assert.Equal(
            "Centro",
            enderecoRepository.BairroRecebidoAtualizar
        );

        Assert.Equal(
            "Blumenau",
            enderecoRepository.CidadeRecebidaAtualizar
        );

        Assert.Equal(
            "89000-000",
            enderecoRepository.CepRecebidoAtualizar
        );
    }

    [Fact]
    public async Task AtualizarAsync_deve_converter_campos_opcionais_vazios_para_null()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoAtualizacaoParaRetornar = true
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            "   ",
            "Centro",
            "Blumenau",
            "   "
        );

        await service.AtualizarAsync(
            1,
            10,
            request
        );

        Assert.Null(
            enderecoRepository.ComplementoRecebidoAtualizar
        );

        Assert.Null(
            enderecoRepository.CepRecebidoAtualizar
        );
    }

    [Fact]
    public async Task AtualizarAsync_nao_deve_chamar_repository_quando_identificacao_estiver_vazia()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.AtualizarAsync(
            1,
            10,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Identificação é obrigatória.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task AtualizarAsync_nao_deve_chamar_repository_quando_logradouro_estiver_vazio()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "",
            "123",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.AtualizarAsync(
            1,
            10,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Logradouro é obrigatório.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task AtualizarAsync_nao_deve_chamar_repository_quando_numero_estiver_vazio()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "",
            null,
            "Centro",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.AtualizarAsync(
            1,
            10,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Número é obrigatório.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task AtualizarAsync_nao_deve_chamar_repository_quando_bairro_estiver_vazio()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "",
            "Blumenau",
            "89000-000"
        );

        var resultado = await service.AtualizarAsync(
            1,
            10,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Bairro é obrigatório.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task AtualizarAsync_nao_deve_chamar_repository_quando_cidade_estiver_vazia()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var request = new AtualizarClienteEnderecoRequest(
            "Casa",
            "Rua das Flores",
            "123",
            null,
            "Centro",
            "",
            "89000-000"
        );

        var resultado = await service.AtualizarAsync(
            1,
            10,
            request
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Validacao,
            resultado.TipoErro
        );

        Assert.Equal(
            "Cidade é obrigatória.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasAtualizarAsync
        );
    }

    [Fact]
    public async Task ExcluirAsync_deve_retornar_sucesso_quando_endereco_for_excluido()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoExclusaoParaRetornar = true
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var resultado = await service.ExcluirAsync(
            1,
            10
        );

        Assert.True(resultado.Sucesso);
        Assert.True(resultado.Valor);
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
        Assert.Equal(
            1,
            enderecoRepository.QuantidadeChamadasExcluirAsync
        );

        Assert.Equal(
            1,
            enderecoRepository.ClienteIdRecebidoExcluir
        );

        Assert.Equal(
            10,
            enderecoRepository.EnderecoIdRecebidoExcluir
        );
    }

    [Fact]
    public async Task ExcluirAsync_deve_retornar_nao_encontrado_quando_endereco_nao_existir()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoExclusaoParaRetornar = false
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var resultado = await service.ExcluirAsync(
            1,
            999
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.NaoEncontrado,
            resultado.TipoErro
        );

        Assert.Equal(
            "Endereço não encontrado.",
            resultado.Erro
        );
    }

    [Fact]
    public async Task ExcluirAsync_deve_retornar_nao_encontrado_quando_cliente_nao_existir()
    {
        var clienteRepository = new FakeClienteRepository();

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var resultado = await service.ExcluirAsync(
            999,
            10
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.NaoEncontrado,
            resultado.TipoErro
        );

        Assert.Equal(
            "Cliente não encontrado.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasExcluirAsync
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_sucesso_quando_endereco_for_reativado()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoReativacaoParaRetornar =
                    ResultadoReativacaoEndereco.Reativado
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var resultado = await service.ReativarAsync(
            1,
            10
        );

        Assert.True(resultado.Sucesso);
        Assert.True(resultado.Valor);
        Assert.Null(resultado.Erro);
        Assert.Null(resultado.TipoErro);
        Assert.Equal(
            1,
            enderecoRepository.QuantidadeChamadasReativarAsync
        );

        Assert.Equal(
            1,
            enderecoRepository.ClienteIdRecebidoReativar
        );

        Assert.Equal(
            10,
            enderecoRepository.EnderecoIdRecebidoReativar
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_nao_encontrado_quando_endereco_nao_existir()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoReativacaoParaRetornar =
                    ResultadoReativacaoEndereco.NaoEncontrado
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var resultado = await service.ReativarAsync(
            1,
            999
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.NaoEncontrado,
            resultado.TipoErro
        );

        Assert.Equal(
            "Endereço não encontrado.",
            resultado.Erro
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_conflito_quando_endereco_ja_estiver_ativo()
    {
        var clienteRepository = new FakeClienteRepository
        {
            ClienteObtidoParaRetornar = new Cliente(
                1,
                "Thiago",
                "47999999999"
            )
        };

        var enderecoRepository =
            new FakeClienteEnderecoRepository
            {
                ResultadoReativacaoParaRetornar =
                    ResultadoReativacaoEndereco.JaAtivo
            };

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var resultado = await service.ReativarAsync(
            1,
            10
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.Conflito,
            resultado.TipoErro
        );

        Assert.Equal(
            "O endereço já está ativo.",
            resultado.Erro
        );
    }

    [Fact]
    public async Task ReativarAsync_deve_retornar_nao_encontrado_quando_cliente_nao_existir()
    {
        var clienteRepository = new FakeClienteRepository();

        var enderecoRepository =
            new FakeClienteEnderecoRepository();

        var service = new ClienteEnderecoService(
            clienteRepository,
            enderecoRepository
        );

        var resultado = await service.ReativarAsync(
            999,
            10
        );

        Assert.False(resultado.Sucesso);

        Assert.Equal(
            TipoErro.NaoEncontrado,
            resultado.TipoErro
        );

        Assert.Equal(
            "Cliente não encontrado.",
            resultado.Erro
        );

        Assert.Equal(
            0,
            enderecoRepository.QuantidadeChamadasReativarAsync
        );
    }
}
