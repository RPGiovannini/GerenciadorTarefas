using GerenciadorTarefas.Domain.Enums;
using MediatR;

namespace GerenciadorTarefas.Application.Tarefas.Requests.CriarTarefa
{
    public class CriarTarefaRequest : IRequest<int>
    {
        //Maximo 100 caracteres
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public EStatusTarefa Status { get; set; }

    }
}


