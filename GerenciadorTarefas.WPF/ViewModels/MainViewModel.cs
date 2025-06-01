using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GerenciadorTarefas.WPF.Enums;
using GerenciadorTarefas.WPF.Models;
using GerenciadorTarefas.WPF.Services;

namespace GerenciadorTarefas.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ITarefaService _tarefaService;

        [ObservableProperty]
        private ObservableCollection<TarefaModel> tarefas = new();

        private ObservableCollection<TarefaModel> _todasTarefas = new();

        [ObservableProperty]
        private TarefaModel? tarefaSelecionada;

        [ObservableProperty]
        private string novoTitulo = string.Empty;

        [ObservableProperty]
        private string novaDescricao = string.Empty;

        [ObservableProperty]
        private int novoStatusIndex = 0;

        [ObservableProperty]
        private TarefaModel tarefaEdicao = new();

        [ObservableProperty]
        private int statusEdicaoIndex = 0;

        [ObservableProperty]
        private Visibility editarTarefaVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private int filtroStatusIndex = 0; // 0 = Todos

        [ObservableProperty]
        private DateTime? filtroData;

        public ETarefaStatus NovoStatus => (ETarefaStatus)(NovoStatusIndex + 1);

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

                _todasTarefas.Clear();
                Tarefas.Clear();
                foreach (var tarefa in tarefasApi)
                {
                    _todasTarefas.Add(tarefa);
                    Tarefas.Add(tarefa);
                }

                // Aplicar filtros após carregar
                AplicarFiltros();

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
                _todasTarefas.Add(tarefaCriada);

                // Reaplicar filtros para mostrar a nova tarefa se ela atender aos critérios
                AplicarFiltros();

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
        private void AbrirEdicaoTarefa()
        {
            if (TarefaSelecionada == null)
            {
                MessageBox.Show("Selecione uma tarefa para editar!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Criar uma cópia da tarefa selecionada para edição
            TarefaEdicao = new TarefaModel
            {
                Id = TarefaSelecionada.Id,
                Titulo = TarefaSelecionada.Titulo,
                Descricao = TarefaSelecionada.Descricao,
                Status = TarefaSelecionada.Status,
                DataCriacao = TarefaSelecionada.DataCriacao,
                DataConclusao = TarefaSelecionada.DataConclusao
            };

            // Configurar o índice do status
            StatusEdicaoIndex = (int)TarefaEdicao.Status - 1;

            // Mostrar o popup de edição
            EditarTarefaVisibility = Visibility.Visible;
        }

        [RelayCommand]
        private void CancelarEdicao()
        {
            EditarTarefaVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        private async Task SalvarEdicaoAsync()
        {
            if (string.IsNullOrWhiteSpace(TarefaEdicao.Titulo))
            {
                MessageBox.Show("O título é obrigatório!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                MensagemStatus = "Atualizando tarefa...";

                // Atualizar o status baseado no índice selecionado
                TarefaEdicao.Status = (ETarefaStatus)(StatusEdicaoIndex + 1);

                // Se o status for Concluído e não tiver data de conclusão, definir para agora
                if (TarefaEdicao.Status == ETarefaStatus.Concluido && !TarefaEdicao.DataConclusao.HasValue)
                {
                    TarefaEdicao.DataConclusao = DateTime.Now;
                }

                var tarefaAtualizada = await _tarefaService.AtualizarTarefaAsync(TarefaEdicao);

                // Atualizar a tarefa na lista completa
                var indexCompleto = _todasTarefas.ToList().FindIndex(t => t.Id == tarefaAtualizada.Id);
                if (indexCompleto >= 0)
                {
                    _todasTarefas[indexCompleto] = tarefaAtualizada;
                }

                // Reaplicar filtros
                AplicarFiltros();

                // Fechar o popup
                EditarTarefaVisibility = Visibility.Collapsed;

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
        private async Task AtualizarTarefaAsync()
        {
            AbrirEdicaoTarefa();
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
                _todasTarefas.Remove(_todasTarefas.FirstOrDefault(t => t.Id == TarefaSelecionada.Id));
                TarefaSelecionada = null;

                // Reaplicar filtros
                AplicarFiltros();

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
                TarefaSelecionada.Status = ETarefaStatus.Concluido;
                TarefaSelecionada.DataConclusao = DateTime.Now.AddMinutes(1);

                // Atualizar na API
                var tarefaAtualizada = await _tarefaService.AtualizarTarefaAsync(TarefaSelecionada);

                // Atualizar na lista completa
                var indexCompleto = _todasTarefas.ToList().FindIndex(t => t.Id == TarefaSelecionada.Id);
                if (indexCompleto >= 0)
                {
                    _todasTarefas[indexCompleto] = tarefaAtualizada;
                }

                // Reaplicar filtros
                AplicarFiltros();

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

        [RelayCommand]
        private void FiltrarTarefas()
        {
            AplicarFiltros();
        }

        [RelayCommand]
        private void LimparFiltros()
        {
            FiltroStatusIndex = 0;
            FiltroData = null;
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            var tarefasFiltradas = _todasTarefas.AsEnumerable();

            // Filtro por Status
            if (FiltroStatusIndex > 0) // 0 = Todos
            {
                var statusFiltro = (ETarefaStatus)FiltroStatusIndex;
                tarefasFiltradas = tarefasFiltradas.Where(t => t.Status == statusFiltro);
            }

            // Filtro por Data
            if (FiltroData.HasValue)
            {
                var dataFiltro = FiltroData.Value.Date;
                tarefasFiltradas = tarefasFiltradas.Where(t =>
                    t.DataCriacao.Date == dataFiltro ||
                    (t.DataConclusao.HasValue && t.DataConclusao.Value.Date == dataFiltro));
            }

            // Atualizar a coleção de tarefas
            Tarefas.Clear();
            foreach (var tarefa in tarefasFiltradas)
            {
                Tarefas.Add(tarefa);
            }

            MensagemStatus = $"{Tarefas.Count} tarefa(s) encontrada(s)";
        }

        public async Task InicializarAsync()
        {
            await CarregarTarefasAsync();
        }
    }
}