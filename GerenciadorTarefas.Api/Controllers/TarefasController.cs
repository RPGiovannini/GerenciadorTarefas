using GerenciadorTarefas.Application.Tarefas.Querys;
using GerenciadorTarefas.Application.Tarefas.Requests.AtualizarTarefa;
using GerenciadorTarefas.Application.Tarefas.Requests.CriarTarefa;
using GerenciadorTarefas.Application.Tarefas.Requests.DeletarTarefa;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GerenciadorTarefas.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de tarefas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]

    public class TarefasController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TarefasController> _logger;

        public TarefasController(IMediator mediator, ILogger<TarefasController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova tarefa
        /// </summary>
        /// <param name="request">Dados da tarefa a ser criada</param>
        /// <returns>Tarefa criada com sucesso</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Cria uma nova tarefa", Description = "Cria uma nova tarefa com os dados fornecidos")]
        public async Task<IActionResult> CriarTarefa([FromBody] CriarTarefaRequest request)
        {
            try
            {
                var result = await _mediator.Send(request);
                //apenas para lembrar de passar o getId quando criar
                return CreatedAtAction(nameof(CriarTarefa), new { id = result }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar a tarefa");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar a tarefa");
            }
        }

        /// <summary>
        /// Obtém todas as tarefas
        /// </summary>
        /// <returns>Lista de tarefas</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Obtém todas as tarefas", Description = "Obtém todas as tarefas cadastradas")]
        public async Task<IActionResult> ObterTarefas()
        {
            try
            {
                var result = await _mediator.Send(new ObterTarefaQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter as tarefas");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obter as tarefas");
            }
        }

        /// <summary>
        /// Obtém uma tarefa específica
        /// </summary>
        /// <param name="id">ID da tarefa a ser obtida</param>
        /// <returns>Tarefa encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Obtém uma tarefa específica", Description = "Obtém uma tarefa específica pelo ID")]
        public async Task<IActionResult> ObterTarefa(int id)
        {
            try
            {
                var result = await _mediator.Send(new ObterTarefaPorIdQuery(id));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a tarefa");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obter a tarefa");
            }
        }

        /// <summary>
        /// Atualiza uma tarefa existente
        /// </summary>
        /// <param name="id">ID da tarefa a ser atualizada</param>
        /// <param name="request">Dados da tarefa a ser atualizada</param>
        /// <returns>Tarefa atualizada com sucesso</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Atualiza uma tarefa existente", Description = "Atualiza uma tarefa existente com os dados fornecidos")]
        public async Task<IActionResult> AtualizarTarefa(int id, [FromBody] AtualizarTarefaRequest request)
        {
            try
            {
                request.Id = id;
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a tarefa");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar a tarefa");
            }
        }

        /// <summary>
        /// Deleta uma tarefa existente
        /// </summary>
        /// <param name="id">ID da tarefa a ser deletada</param>
        /// <returns>Tarefa deletada com sucesso</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Deleta uma tarefa existente", Description = "Deleta uma tarefa existente com o ID fornecido")]
        public async Task<IActionResult> DeletarTarefa(int id)
        {   
            try
            {
                var result = await _mediator.Send(new DeletarTarefaRequest { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar a tarefa");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao deletar a tarefa");
            }
        }
    }
}
