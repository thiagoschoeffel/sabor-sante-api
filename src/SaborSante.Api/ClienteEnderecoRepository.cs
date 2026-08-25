using Npgsql;

public enum ResultadoReativacaoEndereco
{
    Reativado,
    NaoEncontrado,
    JaAtivo
}

public class ClienteEnderecoRepository
    : IClienteEnderecoRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ClienteEnderecoRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<List<ClienteEndereco>> ListarPorClienteAsync(int clienteId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            """
            SELECT
                id,
                cliente_id,
                identificacao,
                logradouro,
                numero,
                complemento,
                bairro,
                cidade,
                cep,
                ativo
            FROM clientes_enderecos
            WHERE cliente_id = @clienteId
                AND ativo = TRUE
            ORDER BY id
            """,
            connection
        );

        command.Parameters.AddWithValue("clienteId", clienteId);

        await using var reader = await command.ExecuteReaderAsync();

        var enderecos = new List<ClienteEndereco>();

        while (await reader.ReadAsync())
        {
            enderecos.Add(
                new ClienteEndereco(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.IsDBNull(5)
                        ? null
                        : reader.GetString(5),
                    reader.GetString(6),
                    reader.GetString(7),
                    reader.IsDBNull(8)
                        ? null
                        : reader.GetString(8),
                    reader.GetBoolean(9)
                )
            );
        }

        return enderecos;
    }

    public async Task<ClienteEndereco> CriarAsync(
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
        await using var connection =
            await _dataSource.OpenConnectionAsync();

        await using var command =
            new NpgsqlCommand(
                """
                INSERT INTO clientes_enderecos
                (
                    cliente_id,
                    identificacao,
                    logradouro,
                    numero,
                    complemento,
                    bairro,
                    cidade,
                    cep
                )
                VALUES
                (
                    @clienteId,
                    @identificacao,
                    @logradouro,
                    @numero,
                    @complemento,
                    @bairro,
                    @cidade,
                    @cep
                )
                RETURNING
                    id,
                    cliente_id,
                    identificacao,
                    logradouro,
                    numero,
                    complemento,
                    bairro,
                    cidade,
                    cep,
                    ativo
                """,
                connection
            );

        command.Parameters.AddWithValue("clienteId", clienteId);
        command.Parameters.AddWithValue("identificacao", identificacao);
        command.Parameters.AddWithValue("logradouro", logradouro);
        command.Parameters.AddWithValue("numero", numero);
        command.Parameters.AddWithValue(
            "complemento",
            complemento is null ? DBNull.Value : complemento
        );
        command.Parameters.AddWithValue("bairro", bairro);
        command.Parameters.AddWithValue("cidade", cidade);
        command.Parameters.AddWithValue(
            "cep",
            cep is null ? DBNull.Value : cep
        );

        await using var reader =
            await command.ExecuteReaderAsync();

        await reader.ReadAsync();

        return new ClienteEndereco(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.IsDBNull(5)
                ? null
                : reader.GetString(5),
            reader.GetString(6),
            reader.GetString(7),
            reader.IsDBNull(8)
                ? null
                : reader.GetString(8),
            reader.GetBoolean(9)
        );
    }

    public async Task<ClienteEndereco?> ObterPorIdAsync(
        int clienteId,
        int enderecoId
    )
    {
        await using var connection =
            await _dataSource.OpenConnectionAsync();

        await using var command =
            new NpgsqlCommand(
                """
                SELECT
                    id,
                    cliente_id,
                    identificacao,
                    logradouro,
                    numero,
                    complemento,
                    bairro,
                    cidade,
                    cep,
                    ativo
                FROM clientes_enderecos
                WHERE id = @enderecoId
                AND cliente_id = @clienteId
                AND ativo = TRUE
                """,
                connection
            );

        command.Parameters.AddWithValue("enderecoId", enderecoId);
        command.Parameters.AddWithValue("clienteId", clienteId);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new ClienteEndereco(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.IsDBNull(5)
                ? null
                : reader.GetString(5),
            reader.GetString(6),
            reader.GetString(7),
            reader.IsDBNull(8)
                ? null
                : reader.GetString(8),
            reader.GetBoolean(9)
        );
    }

    public async Task<bool> AtualizarAsync(
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
        await using var connection =
            await _dataSource.OpenConnectionAsync();

        await using var command =
            new NpgsqlCommand(
                """
                UPDATE clientes_enderecos
                SET identificacao = @identificacao,
                    logradouro = @logradouro,
                    numero = @numero,
                    complemento = @complemento,
                    bairro = @bairro,
                    cidade = @cidade,
                    cep = @cep
                WHERE id = @enderecoId
                AND cliente_id = @clienteId
                AND ativo = TRUE
                """,
                connection
            );

        command.Parameters.AddWithValue("enderecoId", enderecoId);
        command.Parameters.AddWithValue("clienteId", clienteId);
        command.Parameters.AddWithValue("identificacao", identificacao);
        command.Parameters.AddWithValue("logradouro", logradouro);
        command.Parameters.AddWithValue("numero", numero);

        command.Parameters.AddWithValue(
            "complemento",
            complemento is null ? DBNull.Value : complemento
        );

        command.Parameters.AddWithValue("bairro", bairro);
        command.Parameters.AddWithValue("cidade", cidade);

        command.Parameters.AddWithValue(
            "cep",
            cep is null ? DBNull.Value : cep
        );

        var linhasAfetadas =
            await command.ExecuteNonQueryAsync();

        return linhasAfetadas > 0;
    }

    public async Task<bool> ExcluirAsync(
        int clienteId,
        int enderecoId)
    {
        await using var connection =
            await _dataSource.OpenConnectionAsync();

        await using var command =
            new NpgsqlCommand(
                """
                UPDATE clientes_enderecos
                SET ativo = FALSE
                WHERE id = @enderecoId
                AND cliente_id = @clienteId
                AND ativo = TRUE
                """,
                connection
            );

        command.Parameters.AddWithValue("enderecoId", enderecoId);
        command.Parameters.AddWithValue("clienteId", clienteId);

        var linhasAfetadas =
            await command.ExecuteNonQueryAsync();

        return linhasAfetadas > 0;
    }

    public async Task<ResultadoReativacaoEndereco> ReativarAsync(
        int clienteId,
        int enderecoId)
    {
        await using var connection =
            await _dataSource.OpenConnectionAsync();

        await using var command =
            new NpgsqlCommand(
                """
                UPDATE clientes_enderecos
                SET ativo = TRUE
                WHERE id = @enderecoId
                AND cliente_id = @clienteId
                AND ativo = FALSE
                """,
                connection
            );

        command.Parameters.AddWithValue("enderecoId", enderecoId);
        command.Parameters.AddWithValue("clienteId", clienteId);

        var linhasAfetadas =
            await command.ExecuteNonQueryAsync();

        if (linhasAfetadas > 0)
        {
            return ResultadoReativacaoEndereco.Reativado;
        }

        await using var verificarCommand =
            new NpgsqlCommand(
                """
                SELECT ativo
                FROM clientes_enderecos
                WHERE id = @enderecoId
                AND cliente_id = @clienteId
                """,
                connection
            );

        verificarCommand.Parameters.AddWithValue(
            "enderecoId",
            enderecoId
        );

        verificarCommand.Parameters.AddWithValue(
            "clienteId",
            clienteId
        );

        var ativo = await verificarCommand.ExecuteScalarAsync();

        if (ativo is null)
        {
            return ResultadoReativacaoEndereco.NaoEncontrado;
        }

        return ResultadoReativacaoEndereco.JaAtivo;
    }
}
