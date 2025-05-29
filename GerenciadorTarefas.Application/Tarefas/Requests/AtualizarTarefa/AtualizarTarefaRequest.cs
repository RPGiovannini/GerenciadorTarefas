using GerenciadorTarefas.Domain.Enums;
using MediatR;

namespace GerenciadorTarefas.Application.Tarefas.Requests.AtualizarTarefa
{
    public class AtualizarTarefaRequest : IRequest<int>
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public EStatusTarefa Status { get; set; }

    }
}
