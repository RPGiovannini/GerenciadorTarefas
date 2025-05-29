using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GerenciadorTarefas.WPF.Models;
using GerenciadorTarefas.WPF.Services;

namespace GerenciadorTarefas.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ITarefaService _tarefaService;

        [ObservableProperty]
        private ObservableCollection<TarefaModel> tarefas = new();

        [ObservableProperty]
        private TarefaModel? tarefaSelecionada;

        [ObservableProperty]
        private string novoTitulo = string.Empty;

        [ObservableProperty]
        private string novaDescricao = string.Empty;

        [ObservableProperty]
        private int novoStatusIndex = 0;  // Índice 0 = Pendente

        public StatusTarefa NovoStatus => (StatusTarefa)(NovoStatusIndex + 1);  // Mapear: 0->1, 1->2, 2->3

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string mensagemStatus = string.Empty;

        public MainViewModel(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        public MainViewModel() : this(new TarefaService())
        {
        }

        [RelayCommand]
        private async Task CarregarTarefasAsync()
        {
            try
            {
                IsLoading = true;
                MensagemStatus = "Carregando tarefas...";

                var tarefasApi = await _tarefaService.ObterTarefasAsync();

                Tarefas.Clear();
                foreach (var tarefa in tarefasApi)
                {
                    Tarefas.Add(tarefa);
                }

                MensagemStatus = $"{Tarefas.Count} tarefa(s) carregada(s)";
            }
            catch (Exception ex)
            {
                MensagemStatus = $"Erro ao carregar tarefas: {ex.Message}";
                MessageBox.Show($"Erro ao carregar tarefas: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CriarTarefaAsync()
        {
            if (string.IsNullOrWhiteSpace(NovoTitulo))
            {
                MessageBox.Show("O título é obrigatório!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                MensagemStatus = "Criando tarefa...";

                var novaTarefa = new TarefaModel
                {
                    Titulo = NovoTitulo,
                    Descricao = NovaDescricao,
                    Status = NovoStatus,
                    DataCriacao = DateTime.Now
                };

                // Debug: verificar valores
                System.Diagnostics.Debug.WriteLine($"Criando tarefa com Status: {novaTarefa.Status} ({(int)novaTarefa.Status})");

                var tarefaCriada = await _tarefaService.CriarTarefaAsync(novaTarefa);
                Tarefas.Add(tarefaCriada);

                // Limpar campos
                NovoTitulo = string.Empty;
                NovaDescricao = string.Empty;
                NovoStatusIndex = 0;  // Pendente

                MensagemStatus = "Tarefa criada com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemStatus = $"Erro ao criar tarefa: {ex.Message}";
                MessageBox.Show($"Erro ao criar tarefa: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AtualizarTarefaAsync()
        {
            if (TarefaSelecionada == null)
            {
                MessageBox.Show("Selecione uma tarefa para atualizar!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                MensagemStatus = "Atualizando tarefa...";

                var tarefaAtualizada = await _tarefaService.AtualizarTarefaAsync(TarefaSelecionada);

                var index = Tarefas.IndexOf(TarefaSelecionada);
                if (index >= 0)
                {
                    Tarefas[index] = tarefaAtualizada;
                }

                MensagemStatus = "Tarefa atualizada com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemStatus = $"Erro ao atualizar tarefa: {ex.Message}";
                MessageBox.Show($"Erro ao atualizar tarefa: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeletarTarefaAsync()
        {
            if (TarefaSelecionada == null)
            {
                MessageBox.Show("Selecione uma tarefa para deletar!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show(
                $"Tem certeza que deseja deletar a tarefa '{TarefaSelecionada.Titulo}'?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;
                MensagemStatus = "Deletando tarefa...";

                await _tarefaService.DeletarTarefaAsync(TarefaSelecionada.Id);
                Tarefas.Remove(TarefaSelecionada);
                TarefaSelecionada = null;

                MensagemStatus = "Tarefa deletada com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemStatus = $"Erro ao deletar tarefa: {ex.Message}";
                MessageBox.Show($"Erro ao deletar tarefa: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ConcluirTarefaAsync()
        {
            if (TarefaSelecionada == null)
            {
                MessageBox.Show("Selecione uma tarefa para concluir!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                MensagemStatus = "Concluindo tarefa...";

                // Definir status e data de conclusão
                TarefaSelecionada.Status = StatusTarefa.Concluido;
                TarefaSelecionada.DataConclusao = DateTime.Now.AddMinutes(1);

                // Atualizar na API
                var tarefaAtualizada = await _tarefaService.AtualizarTarefaAsync(TarefaSelecionada);

                var index = Tarefas.IndexOf(TarefaSelecionada);
                if (index >= 0)
                {
                    Tarefas[index] = tarefaAtualizada;
                }

                MensagemStatus = "Tarefa concluída com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemStatus = $"Erro ao concluir tarefa: {ex.Message}";
                MessageBox.Show($"Erro ao concluir tarefa: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task InicializarAsync()
        {
            await CarregarTarefasAsync();
        }
    }
}