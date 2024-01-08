using Bogus;
using Bogus.Extensions.Brazil;

namespace AnonimizarDados.Dtos
{
    public class BuilderEntidadeFicticia
    {
        private readonly Faker<EntidadeFicticia> _faker;

        public BuilderEntidadeFicticia()
        {
            _faker = new Faker<EntidadeFicticia>("pt_BR");
        }

        public Faker<EntidadeFicticia> Criar()
        {
            return _faker;
        }

        public BuilderEntidadeFicticia Cep()
        {
            _faker.RuleFor(f => f.Cep, f => f.Address.ZipCode("########"));
            return this;
        }

        public BuilderEntidadeFicticia Cpf()
        {
            _faker.RuleFor(f => f.Cpf, f => f.Person.Cpf(false));
            return this;
        }

        public BuilderEntidadeFicticia Documento()
        {
            _faker.RuleFor(f => f.Documento, f => f.Person.Random.ReplaceNumbers("#########"));
            return this;
        }

        public BuilderEntidadeFicticia Email()
        {
            _faker.RuleFor(f => f.Email, f => f.Person.Email);
            return this;
        }

        public BuilderEntidadeFicticia Endereco()
        {
            _faker.RuleFor(f => f.Endereco, f => f.Address.StreetName());
            return this;
        }

        public BuilderEntidadeFicticia Nome()
        {
            _faker.RuleFor(f => f.Nome, f => f.Person.FullName);
            return this;
        }

        public BuilderEntidadeFicticia Numerico()
        {
            _faker.RuleFor(f => f.Numerico, f => f.Random.Number(1, 1_000));
            return this;
        }

        public BuilderEntidadeFicticia Telefone()
        {
            _faker.RuleFor(f => f.Telefone, f => f.Phone.PhoneNumber("(##) ####-####"));
            return this;
        }
    }
}
