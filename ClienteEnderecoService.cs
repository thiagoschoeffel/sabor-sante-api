public class ClienteEnderecoService
{
    private readonly ClienteRepository _clienteRepository;
    private readonly ClienteEnderecoRepository _enderecoRepository;

    public ClienteEnderecoService(
        ClienteRepository clienteRepository,
        ClienteEnderecoRepository enderecoRepository)
    {
        _clienteRepository = clienteRepository;
        _enderecoRepository = enderecoRepository;
    }

    public async Task<Resultado<ClienteEndereco>> CriarAsync(
        int clienteId,
        CriarClienteEnderecoRequest request)
    {
        var cliente =
            await _clienteRepository.ObterPorIdAsync(clienteId);

        if (cliente is null)
        {
            return Resultado<ClienteEndereco>.Falha(
                "Cliente não encontrado.",
                TipoErro.NaoEncontrado
            );
        }

        var identificacao = request.Identificacao?.Trim();
        var logradouro = request.Logradouro?.Trim();
        var numero = request.Numero?.Trim();
        var complemento = request.Complemento?.Trim();
        var bairro = request.Bairro?.Trim();
        var cidade = request.Cidade?.Trim();
        var cep = request.Cep?.Trim();

        if (string.IsNullOrWhiteSpace(identificacao))
        {
            return Resultado<ClienteEndereco>.Falha(
                "Identificação é obrigatória.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(logradouro))
        {
            return Resultado<ClienteEndereco>.Falha(
                "Logradouro é obrigatório.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(numero))
        {
            return Resultado<ClienteEndereco>.Falha(
                "Número é obrigatório.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(bairro))
        {
            return Resultado<ClienteEndereco>.Falha(
                "Bairro é obrigatório.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(cidade))
        {
            return Resultado<ClienteEndereco>.Falha(
                "Cidade é obrigatória.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(complemento))
        {
            complemento = null;
        }

        if (string.IsNullOrWhiteSpace(cep))
        {
            cep = null;
        }

        var endereco =
            await _enderecoRepository.CriarAsync(
                clienteId,
                identificacao,
                logradouro,
                numero,
                complemento,
                bairro,
                cidade,
                cep
            );

        return Resultado<ClienteEndereco>.Ok(endereco);
    }

    public async Task<Resultado<bool>> AtualizarAsync(
        int clienteId,
        int enderecoId,
        AtualizarClienteEnderecoRequest request)
    {
        var cliente =
            await _clienteRepository.ObterPorIdAsync(clienteId);

        if (cliente is null)
        {
            return Resultado<bool>.Falha(
                "Cliente não encontrado.",
                TipoErro.NaoEncontrado
            );
        }

        var identificacao = request.Identificacao?.Trim();
        var logradouro = request.Logradouro?.Trim();
        var numero = request.Numero?.Trim();
        var complemento = request.Complemento?.Trim();
        var bairro = request.Bairro?.Trim();
        var cidade = request.Cidade?.Trim();
        var cep = request.Cep?.Trim();

        if (string.IsNullOrWhiteSpace(identificacao))
        {
            return Resultado<bool>.Falha(
                "Identificação é obrigatória.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(logradouro))
        {
            return Resultado<bool>.Falha(
                "Logradouro é obrigatório.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(numero))
        {
            return Resultado<bool>.Falha(
                "Número é obrigatório.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(bairro))
        {
            return Resultado<bool>.Falha(
                "Bairro é obrigatório.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(cidade))
        {
            return Resultado<bool>.Falha(
                "Cidade é obrigatória.",
                TipoErro.Validacao
            );
        }

        if (string.IsNullOrWhiteSpace(complemento))
        {
            complemento = null;
        }

        if (string.IsNullOrWhiteSpace(cep))
        {
            cep = null;
        }

        var atualizado =
            await _enderecoRepository.AtualizarAsync(
                clienteId,
                enderecoId,
                identificacao,
                logradouro,
                numero,
                complemento,
                bairro,
                cidade,
                cep
            );

        if (!atualizado)
        {
            return Resultado<bool>.Falha(
                "Endereço não encontrado.",
                TipoErro.NaoEncontrado
            );
        }

        return Resultado<bool>.Ok(true);
    }

    public async Task<Resultado<bool>> ExcluirAsync(
        int clienteId,
        int enderecoId)
    {
        var cliente =
            await _clienteRepository.ObterPorIdAsync(clienteId);

        if (cliente is null)
        {
            return Resultado<bool>.Falha(
                "Cliente não encontrado.",
                TipoErro.NaoEncontrado
            );
        }

        var excluido =
            await _enderecoRepository.ExcluirAsync(
                clienteId,
                enderecoId
            );

        if (!excluido)
        {
            return Resultado<bool>.Falha(
                "Endereço não encontrado.",
                TipoErro.NaoEncontrado
            );
        }

        return Resultado<bool>.Ok(true);
    }

    public async Task<Resultado<bool>> ReativarAsync(
        int clienteId,
        int enderecoId)
    {
        var cliente =
            await _clienteRepository.ObterPorIdAsync(clienteId);

        if (cliente is null)
        {
            return Resultado<bool>.Falha(
                "Cliente não encontrado.",
                TipoErro.NaoEncontrado
            );
        }

        var resultado =
            await _enderecoRepository.ReativarAsync(
                clienteId,
                enderecoId
            );

        if (resultado == ResultadoReativacaoEndereco.NaoEncontrado)
        {
            return Resultado<bool>.Falha(
                "Endereço não encontrado.",
                TipoErro.NaoEncontrado
            );
        }

        if (resultado == ResultadoReativacaoEndereco.JaAtivo)
        {
            return Resultado<bool>.Falha(
                "O endereço já está ativo.",
                TipoErro.Conflito
            );
        }

        return Resultado<bool>.Ok(true);
    }
}
