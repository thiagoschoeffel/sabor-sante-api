public enum TipoErro
{
    Validacao,
    Conflito
}

public record Resultado<T>(
    bool Sucesso,
    T? Valor,
    string? Erro,
    TipoErro? TipoErro
)
{
    public static Resultado<T> Ok(T valor)
    {
        return new Resultado<T>(
            true,
            valor,
            null,
            null
        );
    }

    public static Resultado<T> Falha(string erro, TipoErro tipoErro)
    {
        return new Resultado<T>(
            false,
            default,
            erro,
            tipoErro
        );
    }
}
