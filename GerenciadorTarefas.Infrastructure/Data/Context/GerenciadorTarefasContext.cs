using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Infrastructure.Data.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorTarefas.Infrastructure.Data.Context
{
    public class GerenciadorTarefasContext : DbContext
    {
        public GerenciadorTarefasContext(DbContextOptions<GerenciadorTarefasContext> options) : base(options)
        {
        }

        public DbSet<Tarefa> Tarefas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TarefaConfiguration());
        }
    }
}
