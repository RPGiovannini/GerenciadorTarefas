using GerenciadorTarefas.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GerenciadorTarefas.Infrastructure.Migrations
{
    public static class MigrationHelper
    {
        /// <summary>
        /// Classe para facilitar a geração de migrações através do EF Core Tools
        /// Use: dotnet ef migrations add NomeDaMigracao -p GerenciadorTarefas.Infrastructure -s GerenciadorTarefas.Api
        /// Apos: dotnet ef database update -p GerenciadorTarefas.Infrastructure -s GerenciadorTarefas.Api
        /// </summary>
        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GerenciadorTarefasContext>
        {
            public GerenciadorTarefasContext CreateDbContext(string[] args)
            {
                var basePath = Directory.GetCurrentDirectory();
                var apiDir = Path.Combine(basePath, "../GerenciadorTarefas.Api");
                if (Directory.Exists(apiDir))
                {
                    basePath = apiDir;
                }
                Console.WriteLine($"Diretório base para configuração: {basePath}");
                var configuration = new ConfigurationBuilder()
                                    .SetBasePath(basePath)
                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                    .AddJsonFile($"appsettings.Development.json", optional: true)
                                    .AddEnvironmentVariables()
                                    .Build();
                var builder = new DbContextOptionsBuilder<GerenciadorTarefasContext>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                Console.WriteLine($"String de conexão: {connectionString}");
                builder.UseSqlServer(connectionString);
                return new GerenciadorTarefasContext(builder.Options);
            }
        }
    }
}
