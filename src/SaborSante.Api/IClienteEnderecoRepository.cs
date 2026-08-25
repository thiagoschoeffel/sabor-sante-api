public interface IClienteEnderecoRepository
{
    Task<List<ClienteEndereco>> ListarPorClienteAsync(
        int clienteId
    );

    Task<ClienteEndereco?> ObterPorIdAsync(
        int clienteId,
        int enderecoId
    );

    Task<ClienteEndereco> CriarAsync(
        int clienteId,
        string identificacao,
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string? cep
    );

    Task<bool> AtualizarAsync(
        int clienteId,
        int enderecoId,
        string identificacao,
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string? cep
    );

    Task<bool> ExcluirAsync(
        int clienteId,
        int enderecoId
    );

    Task<ResultadoReativacaoEndereco> ReativarAsync(
        int clienteId,
        int enderecoId
    );
}
