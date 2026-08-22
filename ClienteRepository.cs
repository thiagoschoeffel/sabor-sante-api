using Npgsql;

public enum ResultadoAtualizacaoCliente
{
    Atualizado,
    NaoEncontrado,
    Conflito
}

public class ClienteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ClienteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<List<Cliente>> ListarAsync()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            """
            SELECT 
                id, nome, telefone 
            FROM 
                clientes 
            ORDER BY id
            """,
            connection
        );

        await using var reader = await command.ExecuteReaderAsync();

        var clientes = new List<Cliente>();

        while (await reader.ReadAsync())
        {
            clientes.Add(
                new Cliente(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2)
                )
            );
        }

        return clientes;
    }

    public async Task<Cliente?> ObterPorIdAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            """
            SELECT 
                id, nome, telefone 
            FROM 
                clientes 
            WHERE 
                id = @id
            """,
            connection
        );

        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Cliente(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2)
        );
    }

    public async Task<Cliente?> CriarAsync(string nome, string telefone)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        try
        {
            await using var command = new NpgsqlCommand(
                """
                INSERT INTO clientes (nome, telefone)
                VALUES (@nome, @telefone)
                RETURNING id, nome, telefone
                """,
                connection
            );

            command.Parameters.AddWithValue("nome", nome);
            command.Parameters.AddWithValue("telefone", telefone);

            await using var reader = await command.ExecuteReaderAsync();

            await reader.ReadAsync();

            return new Cliente(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2)
            );
        }
        catch (PostgresException ex)
            when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return null;
        }
    }

    public async Task<ResultadoAtualizacaoCliente> AtualizarAsync(int id, string nome, string telefone)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        try
        {
            await using var command = new NpgsqlCommand(
                """
                UPDATE clientes
                SET nome = @nome,
                    telefone = @telefone
                WHERE id = @id
                """,
                connection
            );

            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("nome", nome);
            command.Parameters.AddWithValue("telefone", telefone);

            var linhasAfetadas = await command.ExecuteNonQueryAsync();

            if (linhasAfetadas == 0)
            {
                return ResultadoAtualizacaoCliente.NaoEncontrado;
            }

            return ResultadoAtualizacaoCliente.Atualizado;
        }
        catch (PostgresException ex)
            when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return ResultadoAtualizacaoCliente.Conflito;
        }
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            """
            DELETE FROM clientes
            WHERE id = @id
            """,
            connection
        );

        command.Parameters.AddWithValue("id", id);

        var linhasAfetadas = await command.ExecuteNonQueryAsync();

        return linhasAfetadas > 0;
    }
}
