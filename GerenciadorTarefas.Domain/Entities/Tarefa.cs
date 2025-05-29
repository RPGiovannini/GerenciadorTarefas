using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GerenciadorTarefas.Domain.Enums;

namespace GerenciadorTarefas.Domain.Entities
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public EStatusTarefa Status { get; set; }

        public Tarefa(string titulo, string? descricao, DateTime dataCriacao, DateTime? dataConclusao, EStatusTarefa status)
        {
            Titulo = titulo;
            Descricao = descricao;
            DataCriacao = dataCriacao;
            DataConclusao = dataConclusao;
            Status = status;
        }
        public void AtualizarTarefa(string titulo, string? descricao, DateTime? dataConclusao, EStatusTarefa novoStatus)
        {
            Titulo = titulo;
            Descricao = descricao;
            DataConclusao = dataConclusao;
            Status = novoStatus;
        }
        public void ConcluirTarefa()
        {
            if (Status == EStatusTarefa.Pendente)
            {
                Status = EStatusTarefa.Concluido;
            }
        }
    }
}
