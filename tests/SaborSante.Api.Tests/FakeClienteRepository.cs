public class FakeClienteRepository : IClienteRepository
{
    public Cliente? ClienteParaRetornar { get; set; }

    public string? NomeRecebido { get; private set; }

    public string? TelefoneRecebido { get; private set; }

    public int QuantidadeChamadasCriarAsync { get; private set; }

    public ResultadoAtualizacaoCliente ResultadoAtualizacaoParaRetornar
    {
        get;
        set;
    } = ResultadoAtualizacaoCliente.Atualizado;

    public int IdRecebidoAtualizar { get; private set; }

    public string? NomeRecebidoAtualizar { get; private set; }

    public string? TelefoneRecebidoAtualizar { get; private set; }

    public int QuantidadeChamadasAtualizarAsync { get; private set; }

    public int QuantidadeChamadasReativarAsync { get; private set; }

    public int IdRecebidoReativar { get; private set; }

    public ResultadoReativacaoCliente ResultadoReativacaoParaRetornar
    {
        get;
        set;
    } = ResultadoReativacaoCliente.Reativado;

    public Task<Cliente?> ObterPorIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Cliente?> CriarAsync(
        string nome,
        string telefone
    )
    {
        QuantidadeChamadasCriarAsync++;

        NomeRecebido = nome;
        TelefoneRecebido = telefone;

        return Task.FromResult(ClienteParaRetornar);
    }

    public Task<ResultadoAtualizacaoCliente> AtualizarAsync(
        int id,
        string nome,
        string telefone
    )
    {
        QuantidadeChamadasAtualizarAsync++;

        IdRecebidoAtualizar = id;
        NomeRecebidoAtualizar = nome;
        TelefoneRecebidoAtualizar = telefone;

        return Task.FromResult(
            ResultadoAtualizacaoParaRetornar
        );
    }

    public Task<ResultadoReativacaoCliente> ReativarAsync(int id)
    {
        QuantidadeChamadasReativarAsync++;
        IdRecebidoReativar = id;

        return Task.FromResult(
            ResultadoReativacaoParaRetornar
        );
    }
}
