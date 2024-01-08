using AnonimizarDados.Dtos;

namespace AnonimizarDados.ValueObjects
{
    public class DefinicaoTabela
    {
        public string NomeEsquema { get; set; }

        public string NomeTabela { get; set; }

        public string NomeCompletoTabela => $"{NomeEsquema}.{NomeTabela}";

        public DefinicaoTabela(TemplateParametrosDeBanco informacoesTabela)
        {
            NomeEsquema = informacoesTabela.Esquema;
            NomeTabela = informacoesTabela.Tabela;
        }
    }
}
