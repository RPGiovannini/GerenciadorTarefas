using GerenciadorTarefas.Domain.Interfaces.Repository;
using GerenciadorTarefas.Infrastructure.Repositorys;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorTarefas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
