using GerenciadorTarefas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorTarefas.Infrastructure.Data.EntitiesConfiguration
{
    public class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Titulo).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Descricao);
            builder.Property(t => t.DataCriacao).IsRequired();
            builder.Property(t => t.DataConclusao);
            builder.Property(t => t.Status).IsRequired();
        }
    }
}
