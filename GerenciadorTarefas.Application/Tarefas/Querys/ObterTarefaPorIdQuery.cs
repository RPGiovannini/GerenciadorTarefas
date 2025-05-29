using GerenciadorTarefas.Domain.Entities;
using MediatR;

namespace GerenciadorTarefas.Application.Tarefas.Querys
{
    public class ObterTarefaPorIdQuery : IRequest<Tarefa>
    {
        public int Id { get; set; }

        public ObterTarefaPorIdQuery(int id)
        {
            Id = id;
        }
    }
}