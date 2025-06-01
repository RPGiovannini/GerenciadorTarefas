using GerenciadorTarefas.WPF.Models;

namespace GerenciadorTarefas.WPF.Services
{
    public interface ITarefaService
    {
        Task<List<TarefaModel>> ObterTarefasAsync();
        Task<TarefaModel> ObterTarefaPorIdAsync(int id);
        Task<TarefaModel> CriarTarefaAsync(TarefaModel tarefa);
        Task<TarefaModel> AtualizarTarefaAsync(TarefaModel tarefa);
        Task<bool> DeletarTarefaAsync(int id);
    }
}
