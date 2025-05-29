using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorTarefas.Application.Tarefas.Querys
{
    public class ObterTarefaQueryHandler : IRequestHandler<ObterTarefaQuery, IEnumerable<Tarefa>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ObterTarefaQueryHandler> _logger;

        public ObterTarefaQueryHandler(IUnitOfWork unitOfWork, ILogger<ObterTarefaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<IEnumerable<Tarefa>> Handle(ObterTarefaQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var tarefas = await _unitOfWork.TarefaRepository.GetAllAsync();
                return tarefas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter as tarefas");
                throw;
            }
        }
    }
}
