using System;
using GerenciadorTarefas.WPF.Enums;
namespace GerenciadorTarefas.WPF.Models
{
    public class TarefaModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public ETarefaStatus Status { get; set; }
    }
}