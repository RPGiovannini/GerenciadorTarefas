namespace GerenciadorTarefas.Domain.Interfaces.Repository
{
    public interface IUnitOfWork
    {
        ITarefaRepository TarefaRepository { get; }
        Task<bool> Commit();
    }
}
