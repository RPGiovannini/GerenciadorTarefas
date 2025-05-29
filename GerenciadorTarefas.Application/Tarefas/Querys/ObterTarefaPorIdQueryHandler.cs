using System.Net;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Exceptions;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorTarefas.Application.Tarefas.Querys
{
    public class ObterTarefaPorIdQueryHandler : IRequestHandler<ObterTarefaPorIdQuery, Tarefa>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ObterTarefaPorIdQueryHandler> _logger;

        public ObterTarefaPorIdQueryHandler(IUnitOfWork unitOfWork, ILogger<ObterTarefaPorIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Tarefa> Handle(ObterTarefaPorIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var tarefa = await _unitOfWork.TarefaRepository.GetByIdAsync(request.Id);

                if (tarefa == null)
                {
                    _logger.LogWarning($"Tarefa com ID {request.Id} não foi encontrada");
                    throw new CustomException(HttpStatusCode.NotFound, $"Tarefa com ID {request.Id} não foi encontrada", new HttpRequestException());
                }
                return tarefa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter a tarefa com ID: {request.Id}");
                throw;
            }
        }
    }
}