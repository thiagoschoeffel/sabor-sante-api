public static class ClienteEnderecoEndpoints
{
    public static void MapClienteEnderecoEndpoints(this WebApplication app)
    {
        app.MapGet("/clientes/{clienteId}/enderecos", async (
            int clienteId,
            ClienteRepository clienteRepository,
            ClienteEnderecoRepository enderecoRepository) =>
        {
            var cliente = await clienteRepository.ObterPorIdAsync(clienteId);

            if (cliente is null)
            {
                return Results.NotFound();
            }

            var enderecos = await enderecoRepository.ListarPorClienteAsync(clienteId);

            return Results.Ok(enderecos);
        });

        app.MapPost(
        "/clientes/{clienteId}/enderecos", async (
            int clienteId,
            CriarClienteEnderecoRequest request,
            ClienteEnderecoService service) =>
        {
            var resultado =
                await service.CriarAsync(clienteId, request);

            if (!resultado.Sucesso)
            {
                return resultado.TipoErro switch
                {
                    TipoErro.Validacao =>
                        Results.BadRequest(new
                        {
                            erro = resultado.Erro
                        }),

                    TipoErro.NaoEncontrado =>
                        Results.NotFound(new
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

            var endereco = resultado.Valor!;

            return Results.Created(
                $"/clientes/{clienteId}/enderecos/{endereco.Id}",
                endereco
            );
        });

        app.MapGet("/clientes/{clienteId}/enderecos/{enderecoId}", async (
            int clienteId,
            int enderecoId,
            ClienteRepository clienteRepository,
            ClienteEnderecoRepository enderecoRepository) =>
        {
            var cliente =
                await clienteRepository.ObterPorIdAsync(clienteId);

            if (cliente is null)
            {
                return Results.NotFound(new
                {
                    erro = "Cliente não encontrado."
                });
            }

            var endereco =
                await enderecoRepository.ObterPorIdAsync(
                    clienteId,
                    enderecoId
                );

            if (endereco is null)
            {
                return Results.NotFound(new
                {
                    erro = "Endereço não encontrado."
                });
            }

            return Results.Ok(endereco);
        });

        app.MapPut("/clientes/{clienteId}/enderecos/{enderecoId}", async (
            int clienteId,
            int enderecoId,
            AtualizarClienteEnderecoRequest request,
            ClienteEnderecoService service) =>
        {
            var resultado =
                await service.AtualizarAsync(
                    clienteId,
                    enderecoId,
                    request
                );

            if (!resultado.Sucesso)
            {
                return resultado.TipoErro switch
                {
                    TipoErro.Validacao =>
                        Results.BadRequest(new
                        {
                            erro = resultado.Erro
                        }),

                    TipoErro.NaoEncontrado =>
                        Results.NotFound(new
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

            return Results.NoContent();
        });

        app.MapDelete("/clientes/{clienteId}/enderecos/{enderecoId}", async (
            int clienteId,
            int enderecoId,
            ClienteEnderecoService service) =>
        {
            var resultado =
                await service.ExcluirAsync(
                    clienteId,
                    enderecoId
                );

            if (!resultado.Sucesso)
            {
                return resultado.TipoErro switch
                {
                    TipoErro.NaoEncontrado =>
                        Results.NotFound(new
                        {
                            erro = resultado.Erro
                        }),

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

            return Results.NoContent();
        });

        app.MapPatch("/clientes/{clienteId}/enderecos/{enderecoId}/reativar", async (
            int clienteId,
            int enderecoId,
            ClienteEnderecoService service) =>
        {
            var resultado =
                await service.ReativarAsync(
                    clienteId,
                    enderecoId
                );

            if (!resultado.Sucesso)
            {
                return resultado.TipoErro switch
                {
                    TipoErro.NaoEncontrado =>
                        Results.NotFound(new
                        {
                            erro = resultado.Erro
                        }),

                    TipoErro.Conflito =>
                        Results.Conflict(new
                        {
                            erro = resultado.Erro
                        }),

                    TipoErro.Validacao =>
                        Results.BadRequest(new
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

            return Results.NoContent();
        });
    }
}
