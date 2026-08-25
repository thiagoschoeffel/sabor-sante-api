using Npgsql;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this WebApplication app)
    {
        app.MapGet("/clientes", async (ClienteRepository repository) =>
        {
            var clientes = await repository.ListarAsync();

            return Results.Ok(clientes);
        });

        app.MapGet("/clientes/{id}", async (int id, ClienteRepository repository) =>
        {
            var cliente = await repository.ObterPorIdAsync(id);

            if (cliente is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(cliente);
        });

        app.MapPost("/clientes", async (CriarClienteRequest request, ClienteService service) =>
        {
            var resultado = await service.CriarAsync(request);

            if (!resultado.Sucesso)
            {
                return resultado.TipoErro switch
                {
                    TipoErro.Validacao =>
                        Results.BadRequest(new
                        {
                            erro = resultado.Erro
                        }),

                    TipoErro.Conflito =>
                        Results.Conflict(new
                        {
                            erro = resultado.Erro
                        }),

                    _ =>
                        Results.BadRequest(new
                        {
                            erro = resultado.Erro
                        })
                };
            }

            var cliente = resultado.Valor!;

            return Results.Created(
                $"/clientes/{cliente.Id}",
                cliente
            );
        });

        app.MapPut("/clientes/{id}", async (int id, AtualizarClienteRequest request, ClienteService service) =>
        {
            var resultado = await service.AtualizarAsync(id, request);

            if (!resultado.Sucesso)
            {
                return resultado.TipoErro switch
                {
                    TipoErro.Validacao =>
                        Results.BadRequest(new
                        {
                            erro = resultado.Erro
                        }),

                    TipoErro.Conflito =>
                        Results.Conflict(new
                        {
                            erro = resultado.Erro
                        }),

                    _ =>
                        Results.BadRequest(new
                        {
                            erro = resultado.Erro
                        })
                };
            }

            if (!resultado.Valor)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });

        app.MapDelete("/clientes/{id}", async (int id, ClienteService service) =>
        {
            var resultado =
                await service.ExcluirAsync(id);

            if (!resultado.Valor)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });

        app.MapPatch("/clientes/{id}/reativar", async (int id, ClienteService service) =>
        {
            var resultado = await service.ReativarAsync(id);

            if (!resultado.Sucesso)
            {
                return resultado.TipoErro switch
                {
                    TipoErro.Conflito =>
                        Results.Conflict(new
                        {
                            erro = resultado.Erro
                        }),

                    _ =>
                        Results.BadRequest(new
                        {
                            erro = resultado.Erro
                        })
                };
            }

            if (!resultado.Valor)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });
    }
}
