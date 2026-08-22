public class ClienteService
{
    private readonly ClienteRepository _repository;

    public ClienteService(ClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Resultado<Cliente>> CriarAsync(CriarClienteRequest request)
    {
        var nome = request.Nome?.Trim();

        var telefone = request.Telefone is null
            ? null
            : new string(
                request.Telefone
                    .Where(char.IsDigit)
                    .ToArray()
            );

        if (string.IsNullOrWhiteSpace(nome))
        {
            return Resultado<Cliente>.Falha(
                "Nome é obrigatório.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(telefone))
        {
            return Resultado<Cliente>.Falha(
                "Telefone é obrigatório.",
                TipoErro.Validacao
            );
        }

        var cliente = await _repository.CriarAsync(nome, telefone);

        if (cliente is null)
        {
            return Resultado<Cliente>.Falha(
                "Já existe um cliente com este telefone.",
                TipoErro.Conflito
            );
        }

        return Resultado<Cliente>.Ok(cliente);
    }

    public async Task<Resultado<bool>> AtualizarAsync(int id, AtualizarClienteRequest request)
    {
        var nome = request.Nome?.Trim();

        var telefone = request.Telefone is null
            ? null
            : new string(
                request.Telefone
                    .Where(char.IsDigit)
                    .ToArray()
            );

        if (string.IsNullOrWhiteSpace(nome))
        {
            return Resultado<bool>.Falha(
                "Nome é obrigatório.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrEmpty(telefone))
        {
            return Resultado<bool>.Falha(
                "Telefone é obrigatório.",
                TipoErro.Validacao
            );
        }

        var resultadoAtualizacao =
            await _repository.AtualizarAsync(
                id,
                nome,
                telefone
            );

        if (resultadoAtualizacao == ResultadoAtualizacaoCliente.Conflito)
        {
            return Resultado<bool>.Falha(
                "Já existe um cliente com este telefone.",
                TipoErro.Conflito
            );
        }

        if (resultadoAtualizacao == ResultadoAtualizacaoCliente.NaoEncontrado)
        {
            return Resultado<bool>.Ok(false);
        }

        return Resultado<bool>.Ok(true);
    }
}
