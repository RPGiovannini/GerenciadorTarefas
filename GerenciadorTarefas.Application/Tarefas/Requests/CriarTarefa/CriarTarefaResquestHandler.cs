using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorTarefas.Application.Tarefas.Requests.CriarTarefa
{
    public class CriarTarefaResquestHandler : IRequestHandler<CriarTarefaRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CriarTarefaResquestHandler> _logger;
        public CriarTarefaResquestHandler(IUnitOfWork unitOfWork, ILogger<CriarTarefaResquestHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<int> Handle(CriarTarefaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var tarefa = new Tarefa(request.Titulo, request.Descricao, request.DataInicio, request.DataFim, request.Status);
                await _unitOfWork.TarefaRepository.AddAsync(tarefa);
                await _unitOfWork.Commit();
                return tarefa.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa");
                throw;
            }
        }
    }
}
