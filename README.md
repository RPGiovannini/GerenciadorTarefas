# Gerenciador de Tarefas

Sistema de gerenciamento de tarefas desenvolvido em C# utilizando ASP.NET Core Web API e WPF, seguindo os princípios de arquitetura em camadas e Clean Code.

## Sobre o Projeto

Este projeto implementa um sistema completo para gerenciamento de tarefas, com backend em ASP.NET Core Web API e frontend desktop em WPF. O sistema permite criar, visualizar, atualizar e excluir tarefas, além de filtrar por status.

## Tecnologias Utilizadas

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Padrão CQRS com MediatR
- Swagger para documentação da API

### Frontend
- WPF (Windows Presentation Foundation)
- MVVM Pattern

## Estrutura do Projeto

O projeto segue uma arquitetura em camadas:

- **GerenciadorTarefas.Domain**: Contém as entidades e regras de negócio
- **GerenciadorTarefas.Application**: Contém a lógica de aplicação (casos de uso)
- **GerenciadorTarefas.Infrastructure**: Implementação dos repositórios e acesso ao banco de dados
- **GerenciadorTarefas.Api**: API RESTful com os endpoints
- **GerenciadorTarefas.WPF**: Interface gráfica desktop

## Pré-requisitos

- .NET 6.0 SDK ou superior
- SQL Server 2019 ou superior
- Visual Studio 2022 (recomendado)

## Como Configurar o Projeto

### Banco de Dados

1. Abra o SQL Server Management Studio ou Azure Data Studio
2. Crie um novo banco de dados chamado `GerenciadorTarefas`
3. Atualize a string de conexão no arquivo `appsettings.json` na pasta `GerenciadorTarefas.Api`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GerenciadorTarefas;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Migrations

Para aplicar as migrações do banco de dados, execute os seguintes comandos no terminal:

1. Execute o comando para aplicar as migrações
```
dotnet ef database update -p GerenciadorTarefas.Infrastructure -s GerenciadorTarefas.Api
```

## Como Executar o Projeto

### API

1. Navegue até a pasta do projeto API
```
cd GerenciadorTarefas.Api
```

2. Execute o comando para iniciar a API
```
dotnet run
```

3. Acesse a documentação Swagger em `https://localhost:5001/swagger`

### Aplicação WPF

1. Abra a solução no Visual Studio
2. Defina o projeto `GerenciadorTarefas.WPF` como projeto de inicialização
3. Pressione F5 para executar a aplicação

## Funcionalidades

### Gerenciamento de Tarefas
- Criar uma nova tarefa
- Visualizar todas as tarefas
- Visualizar uma tarefa específica
- Atualizar uma tarefa existente
- Excluir uma tarefa
- Filtrar tarefas por status (Pendente, Em Andamento, Concluída)

### Modelo de Dados
- **Id**: Identificador único da tarefa (auto incrementado)
- **Título**: Nome da tarefa (obrigatório, máximo 100 caracteres)
- **Descrição**: Detalhes sobre a tarefa (opcional)
- **Data de Criação**: Data em que a tarefa foi criada (gerada automaticamente)
- **Data de Conclusão**: Data em que a tarefa foi concluída (opcional)
- **Status**: Estado atual da tarefa (Pendente, Em Andamento, Concluída)

## Estrutura do Banco de Dados

### Tabela: Tarefas

| Coluna         | Tipo          | Restrições                  |
|----------------|---------------|----------------------------|
| Id             | INT           | PRIMARY KEY, IDENTITY(1,1) |
| Titulo         | NVARCHAR(100) | NOT NULL                   |
| Descricao      | NVARCHAR(MAX) | NULL                       |
| DataCriacao    | DATETIME      | NOT NULL                   |
| DataConclusao  | DATETIME      | NULL                       |
| Status         | INT           | NOT NULL                   |

## API Endpoints

| Método | URL                 | Descrição                      |
|--------|---------------------|--------------------------------|
| GET    | /api/Tarefas        | Obter todas as tarefas         |
| GET    | /api/Tarefas/{id}   | Obter uma tarefa específica    |
| POST   | /api/Tarefas        | Criar uma nova tarefa          |
| PUT    | /api/Tarefas/{id}   | Atualizar uma tarefa existente |
| DELETE | /api/Tarefas/{id}   | Excluir uma tarefa             |

## Regras de Negócio

- O campo Título é obrigatório e tem limite máximo de 100 caracteres
- A Data de Conclusão não pode ser anterior à Data de Criação
- O Status da tarefa segue uma progressão lógica (Pendente → Em Andamento → Concluída)

## Como Executar os Testes

### Testes Unitários

```
cd GerenciadorTarefas.Tests
dotnet test
```

### Testes de Integração

Certifique-se de que o banco de dados de testes está configurado:

```
cd GerenciadorTarefas.IntegrationTests
dotnet test
```

## Troubleshooting

### Problemas com Banco de Dados
- Verifique se a string de conexão está correta
- Certifique-se de que o SQL Server está em execução
- Verifique se as migrações foram aplicadas corretamente

### Problemas com a API
- Verifique os logs em `GerenciadorTarefas.Api/logs`
- Certifique-se de que a porta 5001 está disponível
- Verifique se o certificado HTTPS está válido

### Problemas com a Interface WPF
- Verifique se a API está em execução
- Confirme se a URL da API está configurada corretamente no serviço de tarefas
