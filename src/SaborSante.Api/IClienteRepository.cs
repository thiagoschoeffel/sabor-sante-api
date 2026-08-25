public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(int id);

    Task<Cliente?> CriarAsync(
        string nome,
        string telefone
    );

    Task<ResultadoAtualizacaoCliente> AtualizarAsync(
        int id,
        string nome,
        string telefone
    );

    Task<bool> ExcluirAsync(int id);

    Task<ResultadoReativacaoCliente> ReativarAsync(
        int id
    );
}
