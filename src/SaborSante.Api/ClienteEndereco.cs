public record ClienteEndereco(
    int Id,
    int ClienteId,
    string Identificacao,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string? Cep,
    bool Ativo
);

public record CriarClienteEnderecoRequest(
    string? Identificacao,
    string? Logradouro,
    string? Numero,
    string? Complemento,
    string? Bairro,
    string? Cidade,
    string? Cep
);

public record AtualizarClienteEnderecoRequest(
    string? Identificacao,
    string? Logradouro,
    string? Numero,
    string? Complemento,
    string? Bairro,
    string? Cidade,
    string? Cep
);