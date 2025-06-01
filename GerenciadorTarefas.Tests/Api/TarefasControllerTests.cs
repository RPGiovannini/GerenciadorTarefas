using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GerenciadorTarefas.Api.Controllers;
using GerenciadorTarefas.Application.Tarefas.Querys;
using GerenciadorTarefas.Application.Tarefas.Requests.CriarTarefa;
using GerenciadorTarefas.Application.Tarefas.Requests.DeletarTarefa;
using GerenciadorTarefas.Application.Tarefas.Requests.AtualizarTarefa;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Enums;
using GerenciadorTarefas.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Net;

namespace GerenciadorTarefas.Tests.Api
{
    public class TarefasControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<TarefasController>> _loggerMock;
        private readonly TarefasController _controller;

        public TarefasControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<TarefasController>>();
            _controller = new TarefasController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ObterTarefas_QuandoExistemTarefas_DeveRetornarOkComListaDeTarefas()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                new Tarefa("Tarefa 1", "Descrição 1", DateTime.Now, null, EStatusTarefa.Pendente) { Id = 1 },
                new Tarefa("Tarefa 2", "Descrição 2", DateTime.Now, null, EStatusTarefa.EmAndamento) { Id = 2 }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterTarefaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefas);

            // Act
            var result = await _controller.ObterTarefas();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(tarefas);
        }

        [Fact]
        public async Task ObterTarefas_QuandoOcorreErro_DeveRetornarStatusCode500()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterTarefaQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var result = await _controller.ObterTarefas();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().Be("Erro ao obter as tarefas");
        }

        [Fact]
        public async Task ObterTarefa_QuandoTarefaExiste_DeveRetornarOkComTarefa()
        {
            // Arrange
            var tarefa = new Tarefa("Tarefa 1", "Descrição 1", DateTime.Now, null, EStatusTarefa.Pendente) { Id = 1 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterTarefaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefa);

            // Act
            var result = await _controller.ObterTarefa(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(tarefa);
        }

        [Fact]
        public async Task ObterTarefa_QuandoTarefaNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterTarefaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CustomException(HttpStatusCode.NotFound, "Tarefa não encontrada", new Exception()));

            // Act
            var result = await _controller.ObterTarefa(1);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task CriarTarefa_QuandoDadosValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var request = new CriarTarefaRequest
            {
                Titulo = "Nova Tarefa",
                Descricao = "Descrição da tarefa",
                DataInicio = DateTime.Now,
                Status = EStatusTarefa.Pendente
            };

            var tarefaId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<CriarTarefaRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefaId);

            // Act
            var result = await _controller.CriarTarefa(request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.ActionName.Should().Be(nameof(TarefasController.ObterTarefa));
            createdResult.RouteValues["id"].Should().Be(tarefaId);
            createdResult.Value.Should().Be(tarefaId);
        }

        [Fact]
        public async Task AtualizarTarefa_QuandoTarefaExiste_DeveRetornarOkComId()
        {
            // Arrange
            var tarefaId = 1;
            var request = new AtualizarTarefaRequest
            {
                Titulo = "Tarefa Atualizada",
                Descricao = "Descrição atualizada",
                DataCriacao = DateTime.Now,
                Status = EStatusTarefa.EmAndamento
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<AtualizarTarefaRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefaId);

            // Act
            var result = await _controller.AtualizarTarefa(tarefaId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(tarefaId);

            // Verifica se o ID foi definido na requisição
            request.Id.Should().Be(tarefaId);
        }

        [Fact]
        public async Task AtualizarTarefa_QuandoTarefaNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var tarefaId = 1;
            var request = new AtualizarTarefaRequest
            {
                Titulo = "Tarefa Atualizada",
                Descricao = "Descrição atualizada",
                DataCriacao = DateTime.Now,
                Status = EStatusTarefa.EmAndamento
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<AtualizarTarefaRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CustomException(HttpStatusCode.NotFound, "Tarefa não encontrada", new Exception()));

            // Act
            var result = await _controller.AtualizarTarefa(tarefaId, request);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task DeletarTarefa_QuandoTarefaExiste_DeveRetornarOkComTrue()
        {
            // Arrange
            var tarefaId = 1;

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeletarTarefaRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletarTarefa(tarefaId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(true);
        }

        [Fact]
        public async Task DeletarTarefa_QuandoTarefaNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var tarefaId = 1;

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeletarTarefaRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CustomException(HttpStatusCode.NotFound, "Tarefa não encontrada", new Exception()));

            // Act
            var result = await _controller.DeletarTarefa(tarefaId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}