using GerenciadorTarefas.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorTarefas.Infrastructure.Data
{
    public static class DataBaseExtensions
    {
        public static IServiceCollection AddDataBase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GerenciadorTarefasContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                b => b.MigrationsAssembly(typeof(GerenciadorTarefasContext).Assembly.FullName)));
            return services;
        }
    }
}
