using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Enums;
using MediatR;

namespace GerenciadorTarefas.Application.Tarefas.Querys
{
    public class ObterTarefaQuery : IRequest<IEnumerable<Tarefa>>
    {
    }
}
