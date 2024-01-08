using AnonimizarDados.Dtos;
using AnonimizarDados.Enums;
using System;

namespace AnonimizarDados.Servicos;

public static class RegraService
{
    public static void GerarBuilder(BuilderEntidadeFicticia builder, RegrasEnum regraEnum)
    {
        switch (regraEnum)
        {
            case RegrasEnum.Cep:
                builder.Cep();
                break;
            case RegrasEnum.Cpf:
                builder.Cpf();
                break;
            case RegrasEnum.Documento:
                builder.Documento();
                break;
            case RegrasEnum.Email:
                builder.Email();
                break;
            case RegrasEnum.Endereco:
                builder.Endereco();
                break;
            case RegrasEnum.Nome:
                builder.Nome();
                break;
            case RegrasEnum.Numerico:
                builder.Numerico();
                break;
            case RegrasEnum.Telefone:
                builder.Telefone();
                break;
            case RegrasEnum.Infefinido:
            default:
                throw new NotImplementedException("Regra não implementada para builder.");
        }
    }

    public static object ObterValor(EntidadeFicticia entidade, RegrasEnum regraEnum)
    {
        return regraEnum switch
        {
            RegrasEnum.Cep => entidade.Cep,
            RegrasEnum.Cpf => entidade.Cpf,
            RegrasEnum.Documento => entidade.Documento,
            RegrasEnum.Email => entidade.Email,
            RegrasEnum.Endereco => entidade.Endereco,
            RegrasEnum.Nome => entidade.Nome,
            RegrasEnum.Numerico => entidade.Numerico,
            RegrasEnum.Telefone => entidade.Telefone,
            _ => throw new NotImplementedException("Regra não implementada para obter valor."),
        };
    }
}