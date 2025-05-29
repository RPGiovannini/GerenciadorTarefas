# Gerenciador de Tarefas - Frontend WPF

Este é o frontend em WPF para o sistema de gerenciamento de tarefas.

## Características

- **Interface Moderna**: Utiliza Material Design para uma experiência visual moderna
- **Padrão MVVM**: Implementado com CommunityToolkit.Mvvm para separação de responsabilidades
- **Comunicação com API**: Integração completa com a API REST
- **Operações CRUD**: Criar, listar, atualizar e deletar tarefas
- **Interface Responsiva**: Layout adaptável com cards e componentes Material Design

## Funcionalidades

### Lista de Tarefas

- Visualização em DataGrid com todas as tarefas
- Colunas: ID, Título, Status, Data de Criação, Data de Conclusão
- Seleção de tarefas para operações

### Criação de Tarefas

- Formulário lateral para criar novas tarefas
- Campos: Título (obrigatório), Descrição (opcional), Status
- Validação de campos obrigatórios

### Operações

- **Atualizar**: Modifica uma tarefa selecionada
- **Concluir**: Marca uma tarefa como concluída
- **Deletar**: Remove uma tarefa (com confirmação)
- **Atualizar Lista**: Recarrega as tarefas da API

### Status Bar

- Indicador de carregamento
- Mensagens de status das operações
- Feedback visual para o usuário

## Pré-requisitos

1. **.NET 9.0** ou superior
2. **API em execução** na porta 5001 (HTTPS)
3. **Pacotes NuGet**:
   - CommunityToolkit.Mvvm (8.2.2)
   - MaterialDesignThemes (5.1.0)
   - Newtonsoft.Json (13.0.3)

## Como Executar

### 1. Iniciar a API

Primeiro, certifique-se de que a API está rodando:

```bash
cd ../GerenciadorTarefas.Api
dotnet run
```

A API deve estar disponível em `https://localhost:5001`

### 2. Executar o Frontend WPF

```bash
cd GerenciadorTarefas.WPF
dotnet run
```

Ou compile e execute:

```bash
dotnet build
dotnet run
```

## Estrutura do Projeto

```
GerenciadorTarefas.WPF/
├── Models/
│   └── TarefaModel.cs          # Modelo de dados
├── ViewModels/
│   └── MainViewModel.cs        # ViewModel principal (MVVM)
├── Services/
│   └── TarefaService.cs        # Serviço para comunicação com API
├── Views/
│   └── (futuras views)
├── MainWindow.xaml             # Interface principal
├── MainWindow.xaml.cs          # Code-behind
├── App.xaml                    # Configuração da aplicação
└── App.xaml.cs                 # Inicialização da aplicação
```

## Configuração da API

O frontend está configurado para se comunicar com a API em:

- **URL Base**: `https://localhost:5001/api/tarefas`

Se a API estiver rodando em uma porta diferente, altere a URL no arquivo `Services/TarefaService.cs`:

```csharp
private readonly string _baseUrl = "https://localhost:PORTA/api/tarefas";
```

## Tecnologias Utilizadas

- **WPF (.NET 9.0)**: Framework de interface
- **Material Design**: Biblioteca de componentes visuais
- **MVVM Pattern**: Padrão arquitetural
- **CommunityToolkit.Mvvm**: Implementação MVVM
- **HttpClient**: Comunicação HTTP
- **Newtonsoft.Json**: Serialização JSON

## Troubleshooting

### Erro de Conexão com API

- Verifique se a API está rodando
- Confirme a URL e porta da API
- Verifique se o certificado HTTPS está válido

### Problemas de Interface

- Certifique-se de que o Material Design está configurado corretamente
- Verifique se todos os pacotes NuGet foram restaurados

### Erros de Compilação

- Execute `dotnet restore` para restaurar pacotes
- Verifique se está usando .NET 9.0 ou superior
