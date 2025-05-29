using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using GerenciadorTarefas.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> DeleteAsync(Tarefa tarefa)
        {
            if (tarefa == null)
            {
                return false;
            }
            _context.Tarefas.Remove(tarefa);
            return true;
        }
        public async Task<IEnumerable<Tarefa>> GetAllAsync()
        {
            return await _context.Tarefas.ToListAsync();
        }

        public async Task<Tarefa> GetByIdAsync(int id)
        {
            return await _context.Tarefas.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Tarefa tarefa)
        {
            _context.Tarefas.Update(tarefa);
            return true;
        }
    }
}
