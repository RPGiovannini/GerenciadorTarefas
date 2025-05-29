using GerenciadorTarefas.Application.Tarefas.Requests.CriarTarefa;
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
    }
}
