using AnonimizarDados.Enums;
using Bogus;
using Bogus.Extensions.Brazil;
using System;

namespace AnonimizarDados.Servicos;

public static class RegraService
{
    public static object GerarValor(RegrasEnum regraEnum)
    {
        switch (regraEnum)
        {
            case RegrasEnum.Cep:
                return new Faker("pt_BR").Address.ZipCode("########");
            case RegrasEnum.Cpf:
                return new Faker("pt_BR").Person.Cpf(false);
            case RegrasEnum.Documento:
                return new Faker("pt_BR").Person.Random.ReplaceNumbers("#########");
            case RegrasEnum.Email:
                return new Faker("pt_BR").Person.Email;
            case RegrasEnum.Endereco:
                return new Faker("pt_BR").Address.StreetName();
            case RegrasEnum.Nome:
                return new Faker("pt_BR").Person.FullName;
            case RegrasEnum.Numerico:
                return new Faker("pt_BR").Random.Number(1_000);
            case RegrasEnum.Telefone:
                return new Faker("pt_BR").Phone.PhoneNumber("(##) ####-####");
            default:
                throw new NotImplementedException("Regra não implementada.");
        }
    }
}