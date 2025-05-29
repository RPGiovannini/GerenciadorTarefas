using GerenciadorTarefas.Domain.Entities;

namespace GerenciadorTarefas.Domain.Interfaces.Repository
{
    public interface ITarefaRepository
    {
        Task<Tarefa> GetByIdAsync(int id);
        Task<IEnumerable<Tarefa>> GetAllAsync();
        Task<int> AddAsync(Tarefa tarefa);
        Task<bool> UpdateAsync(Tarefa tarefa);
        Task<bool> DeleteAsync(int id);


    }
}
