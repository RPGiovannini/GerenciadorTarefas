using System;

namespace GerenciadorTarefas.WPF.Models
{
    public class TarefaModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public StatusTarefa Status { get; set; }
    }

    public enum StatusTarefa
    {
        Pendente = 1,
        EmAndamento = 2,
        Concluido = 3
    }
}