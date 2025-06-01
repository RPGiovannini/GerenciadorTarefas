using MediatR;
using GerenciadorTarefas.Domain.Interfaces;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using GerenciadorTarefas.Domain.Exceptions;
using System.Net;
using GerenciadorTarefas.Domain.Enums;
namespace GerenciadorTarefas.Application.Tarefas.Requests.AtualizarTarefa
{
    public class AtualizarTarefaRequestHandler : IRequestHandler<AtualizarTarefaRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AtualizarTarefaRequestHandler> _logger;
        public AtualizarTarefaRequestHandler(IUnitOfWork unitOfWork, ILogger<AtualizarTarefaRequestHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<int> Handle(AtualizarTarefaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var tarefa = await _unitOfWork.TarefaRepository.GetByIdAsync(request.Id.Value);
                if (tarefa == null)
                {
                    _logger.LogError($"Tarefa {request.Id} não encontrada");
                    throw new CustomException(HttpStatusCode.NotFound, $"Tarefa {request.Id} não encontrada", new HttpRequestException());
                }

                tarefa.AtualizarTarefa(request.Titulo, request.Descricao, request.DataConclusao, request.Status);

                await _unitOfWork.TarefaRepository.UpdateAsync(tarefa);
                await _unitOfWork.Commit();

                _logger.LogInformation($"Tarefa {tarefa.Id} atualizada com sucesso");
                return tarefa.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar a tarefa {request.Id}", request.Id);
                throw;
            }
        }

    }
}
