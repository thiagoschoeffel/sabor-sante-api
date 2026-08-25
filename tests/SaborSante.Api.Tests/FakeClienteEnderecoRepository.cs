public class FakeClienteEnderecoRepository
    : IClienteEnderecoRepository
{
    public ClienteEndereco? EnderecoParaRetornar { get; set; }

    public int ClienteIdRecebidoCriar { get; private set; }

    public string? IdentificacaoRecebidaCriar { get; private set; }

    public string? LogradouroRecebidoCriar { get; private set; }

    public string? NumeroRecebidoCriar { get; private set; }

    public string? ComplementoRecebidoCriar { get; private set; }

    public string? BairroRecebidoCriar { get; private set; }

    public string? CidadeRecebidaCriar { get; private set; }

    public string? CepRecebidoCriar { get; private set; }

    public int QuantidadeChamadasCriarAsync { get; private set; }

    public bool ResultadoAtualizacaoParaRetornar { get; set; } = true;

    public int QuantidadeChamadasAtualizarAsync { get; private set; }

    public int ClienteIdRecebidoAtualizar { get; private set; }

    public int EnderecoIdRecebidoAtualizar { get; private set; }

    public string? IdentificacaoRecebidaAtualizar { get; private set; }

    public string? LogradouroRecebidoAtualizar { get; private set; }

    public string? NumeroRecebidoAtualizar { get; private set; }

    public string? ComplementoRecebidoAtualizar { get; private set; }

    public string? BairroRecebidoAtualizar { get; private set; }

    public string? CidadeRecebidaAtualizar { get; private set; }

    public string? CepRecebidoAtualizar { get; private set; }

    public bool ResultadoExclusaoParaRetornar { get; set; } = true;

    public int QuantidadeChamadasExcluirAsync { get; private set; }

    public int ClienteIdRecebidoExcluir { get; private set; }

    public int EnderecoIdRecebidoExcluir { get; private set; }

    public ResultadoReativacaoEndereco ResultadoReativacaoParaRetornar
    {
        get;
        set;
    } = ResultadoReativacaoEndereco.Reativado;

    public int QuantidadeChamadasReativarAsync { get; private set; }

    public int ClienteIdRecebidoReativar { get; private set; }

    public int EnderecoIdRecebidoReativar { get; private set; }

    public Task<List<ClienteEndereco>> ListarPorClienteAsync(
        int clienteId)
    {
        throw new NotImplementedException();
    }

    public Task<ClienteEndereco?> ObterPorIdAsync(
        int clienteId,
        int enderecoId)
    {
        throw new NotImplementedException();
    }

    public Task<ClienteEndereco> CriarAsync(
        int clienteId,
        string identificacao,
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string? cep
    )
    {
        QuantidadeChamadasCriarAsync++;

        ClienteIdRecebidoCriar = clienteId;
        IdentificacaoRecebidaCriar = identificacao;
        LogradouroRecebidoCriar = logradouro;
        NumeroRecebidoCriar = numero;
        ComplementoRecebidoCriar = complemento;
        BairroRecebidoCriar = bairro;
        CidadeRecebidaCriar = cidade;
        CepRecebidoCriar = cep;

        return Task.FromResult(
            EnderecoParaRetornar!
        );
    }

    public Task<bool> AtualizarAsync(
        int clienteId,
        int enderecoId,
        string identificacao,
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string? cep)
    {
        QuantidadeChamadasAtualizarAsync++;

        ClienteIdRecebidoAtualizar = clienteId;
        EnderecoIdRecebidoAtualizar = enderecoId;
        IdentificacaoRecebidaAtualizar = identificacao;
        LogradouroRecebidoAtualizar = logradouro;
        NumeroRecebidoAtualizar = numero;
        ComplementoRecebidoAtualizar = complemento;
        BairroRecebidoAtualizar = bairro;
        CidadeRecebidaAtualizar = cidade;
        CepRecebidoAtualizar = cep;

        return Task.FromResult(
            ResultadoAtualizacaoParaRetornar
        );
    }

    public Task<bool> ExcluirAsync(
        int clienteId,
        int enderecoId)
    {
        QuantidadeChamadasExcluirAsync++;

        ClienteIdRecebidoExcluir = clienteId;
        EnderecoIdRecebidoExcluir = enderecoId;

        return Task.FromResult(
            ResultadoExclusaoParaRetornar
        );
    }

    public Task<ResultadoReativacaoEndereco> ReativarAsync(
        int clienteId,
        int enderecoId)
    {
        QuantidadeChamadasReativarAsync++;

        ClienteIdRecebidoReativar = clienteId;
        EnderecoIdRecebidoReativar = enderecoId;

        return Task.FromResult(
            ResultadoReativacaoParaRetornar
        );
    }
}
