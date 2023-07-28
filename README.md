# Anonimizador de dados

Ferramenta com o propósito de anonimizar os dados de um bando de dados SQL Server a fim de atender os requisitos da LGPD.

A ferramenta será utilizada no servidor de banco do cliente em uma cópia do banco de produção/homologação e após os dados forem anonimizados, será feito um backup deste banco tratado para ser distribuído aos desenvolvedores, para que os mesmos sigam com suas demandas.

## Funcionamento

A ferramenta irá percorrer determidas tabelas do banco de dados que forem especificadas no arquivo de configuração `appsettings.json` e executar quatro ações:

1. Exclusão de registros com o comando "DELETE FROM"

2. Exclusão de registros com o comando "TRUNCATE TABLE"

3. Atualização de registros com o comando "UPDATE" `sem WHERE` para um valor fixo.

4. Atualização de registros com o comando "UPDATE" `registro a registro` seguindo uma regra especificada para randomização da informação.

## Estrutura do arquivo appsettings.json

`ConnectionStrings.Banco` : string de conexão do banco de dados a ser anonimizado.

`Parametros.Excluir` : parâmetros com a finalidade de executar a limpeza de uma determinada tabela por meio do comando "DELETE FROM".

```
Esquema: nome do schema da tabla

Tabela: nome da tabela

Prioridade: define uma prioridade na execução das exclusões dos dados
- quanto menor o valor, mais prioritário
- quanto maior o valor, menos prioritário
```

`Parametros.Truncar` : parâmetros com a finalidade de executar a limpeza de uma determinada tabela por meio do comando "TRUNCATE TABLE"

```
Esquema: nome do schema da tabla

Tabela: nome da tabela

Prioridade: define uma prioridade na execução das exclusões dos dados
- quanto menor o valor, mais prioritário
- quanto maior o valor, menos prioritário
```

`Parametros.AtualizarPorRegra` : parâmetros com a finalidade de atualizar os registros por meio de uma regra.

```
Esquema: nome do schema da tabla

Tabela: nome da tabela

Coluna: nome da coluna onde será aplicado o comando update

Valor: valor fixo a ser usado no comando update
```

`Parametros.AtualizarPorValor` : parâmetros com a finalidade de atualizar os registros por meio de um valor fixo.

```
Esquema: nome do schema da tabla

Tabela: nome da tabela

ColunaId: nome da coluna PK da tabela. Usado para ordenação dos dados e identificação para o update unitário

Coluna: nome da coluna onde será aplicado o comando update

Regra: especifica qual regra a ser usada para a geração do valor fictício. Mais detalhes, consultar o tópico "Regras"
```

## Regras

Segue a relação das regras disponíveis para uso:

| Regra | Valor resultante |
| - | - |
| Cep | Gera um CEP de 8 digitos sem máscara. |
| Cpf | Gera um CPF aleatório válido sem máscara. |
| Documento | Gera um número de documento de 9 digitos sem máscara. |
| Email | Gera um e-mail fictício de dóminios conhecidos (live.com / hotmail.com / etc). |
| Endereco | Gera um nome de endereço aleatório. |
| Nome | Gera um nome de pessoa aleatório. |
| Numerico | Gera um valor numérico entre 1 e 1.000 |
| Telefone | Gera um telefone no formato (##) ####-####. |

_Lib utilizada para geração dos dados fictícios: [Bogus](https://github.com/bchavez/Bogus)_

## Validações

1. Parâmetro não informado

2. String de conexão não informada

3. Teste conexão com o banco de dados

4. Banco de dados deve possuir "_RANDOM" em seu nome
