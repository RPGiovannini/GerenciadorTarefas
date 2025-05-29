using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using GerenciadorTarefas.Infrastructure.Data.Context;

namespace GerenciadorTarefas.Infrastructure.Repositorys
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly GerenciadorTarefasContext _context;

        public TarefaRepository(GerenciadorTarefasContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Tarefa tarefa)
        {
            await _context.Tarefas.AddAsync(tarefa);
            return tarefa.Id;
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tarefa>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Tarefa> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Tarefa tarefa)
        {
            throw new NotImplementedException();
        }
    }
}
