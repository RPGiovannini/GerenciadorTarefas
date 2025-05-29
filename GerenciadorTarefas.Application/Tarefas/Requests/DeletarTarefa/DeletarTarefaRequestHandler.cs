using GerenciadorTarefas.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorTarefas.Application.Tarefas.Requests.DeletarTarefa
{
    public class DeletarTarefaRequestHandler : IRequestHandler<DeletarTarefaRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeletarTarefaRequestHandler> _logger;

        public DeletarTarefaRequestHandler(IUnitOfWork unitOfWork, ILogger<DeletarTarefaRequestHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<bool> Handle(DeletarTarefaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var tarefa = await _unitOfWork.TarefaRepository.GetByIdAsync(request.Id);
                if (tarefa == null)
                {
                    _logger.LogWarning($"Tarefa não encontrada: {request.Id}");
                    throw new InvalidOperationException($"Tarefa não encontrada: {request.Id}");
                }
                await _unitOfWork.TarefaRepository.DeleteAsync(tarefa);
                await _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao deletar a tarefa: {request.Id}");
                throw;
            }
        }
    }
}
