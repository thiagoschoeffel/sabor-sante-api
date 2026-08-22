public record Cliente(int Id, string Nome, string Telefone);

public record CriarClienteRequest(string? Nome, string? Telefone);

public record AtualizarClienteRequest(string? Nome, string? Telefone);
