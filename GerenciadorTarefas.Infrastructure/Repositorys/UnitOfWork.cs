using GerenciadorTarefas.Domain.Interfaces.Repository;
using GerenciadorTarefas.Infrastructure.Data.Context;

namespace GerenciadorTarefas.Infrastructure.Repositorys
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GerenciadorTarefasContext _context;
        public ITarefaRepository TarefaRepository { get; private set; }

        public UnitOfWork(GerenciadorTarefasContext context)
        {
            _context = context; 
            TarefaRepository = new TarefaRepository(_context);
        }

        public async Task<bool> Commit()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
