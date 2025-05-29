using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using GerenciadorTarefas.Infrastructure.Data;
using GerenciadorTarefas.Application;
using GerenciadorTarefas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
// Configuração das URLs
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5000); // HTTP
    serverOptions.ListenLocalhost(5001, options => options.UseHttps()); // HTTPS
});
// Configurar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Configurar para exibir mensagens de erro personalizadas
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return new BadRequestObjectResult(new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            title = "Um ou mais erros de validação ocorreram.",
            status = 400,
            errors = errors,
            traceId = System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
        });
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gerenciador de Tarefas",
        Version = "v1",
        Description = "API para gerenciamento de tarefas",
        Contact = new OpenApiContact
        {
            Name = "Rafael",
            Email = "rafaelpereiragiovannini@hotmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Uso Interno"
        }
    });

    // Configurar a leitura de comentários XML para o Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Adicionar comentários XML do projeto Application
    var xmlApplicationFile = $"GerenciadorTarefas.Application.xml";
    var xmlApplicationPath = Path.Combine(AppContext.BaseDirectory, xmlApplicationFile);
    if (File.Exists(xmlApplicationPath))
    {
        c.IncludeXmlComments(xmlApplicationPath);
    }

    // Adicionar comentários XML do projeto Domain
    var xmlDomainFile = $"GerenciadorTarefas.Domain.xml";
    var xmlDomainPath = Path.Combine(AppContext.BaseDirectory, xmlDomainFile);
    if (File.Exists(xmlDomainPath))
    {
        c.IncludeXmlComments(xmlDomainPath);
    }

    // Adicionar suporte para anotações do Swashbuckle
    c.EnableAnnotations();
});
builder.Services.AddDataBase(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddRepository();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gerenciador de Tarefas v1"));
}


app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
