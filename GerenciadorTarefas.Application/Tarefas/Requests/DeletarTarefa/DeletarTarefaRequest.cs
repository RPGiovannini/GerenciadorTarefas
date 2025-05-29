using MediatR;

namespace GerenciadorTarefas.Application.Tarefas.Requests.DeletarTarefa
{
    public class DeletarTarefaRequest : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
