using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GerenciadorTarefas.Application.Tarefas.Requests.AtualizarTarefa;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Enums;
using GerenciadorTarefas.Domain.Exceptions;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GerenciadorTarefas.Tests.Application
{
    public class AtualizarTarefaRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITarefaRepository> _tarefaRepositoryMock;
        private readonly Mock<ILogger<AtualizarTarefaRequestHandler>> _loggerMock;
        private readonly AtualizarTarefaRequestHandler _handler;

        public AtualizarTarefaRequestHandlerTests()
        {
            _tarefaRepositoryMock = new Mock<ITarefaRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AtualizarTarefaRequestHandler>>();

            _unitOfWorkMock.Setup(uow => uow.TarefaRepository).Returns(_tarefaRepositoryMock.Object);
            _unitOfWorkMock.Setup(uow => uow.Commit()).ReturnsAsync(true);

            _handler = new AtualizarTarefaRequestHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_QuandoTarefaExiste_DeveAtualizarTarefaERetornarId()
        {
            // Arrange
            var tarefaId = 1;
            var tarefa = new Tarefa("Título Original", "Descrição Original", DateTime.Now, null, EStatusTarefa.Pendente)
            {
                Id = tarefaId
            };

            var request = new AtualizarTarefaRequest
            {
                Id = tarefaId,
                Titulo = "Novo Título",
                Descricao = "Nova Descrição",
                DataCriacao = DateTime.Now,
                DataConclusao = DateTime.Now.AddDays(1),
                Status = EStatusTarefa.Concluido
            };

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId)).ReturnsAsync(tarefa);
            _tarefaRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Tarefa>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(tarefaId);
            _tarefaRepositoryMock.Verify(repo => repo.GetByIdAsync(tarefaId), Times.Once);
            _tarefaRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Tarefa>(t =>
                t.Id == tarefaId &&
                t.Titulo == request.Titulo &&
                t.Descricao == request.Descricao &&
                t.DataConclusao == request.DataConclusao &&
                t.Status == request.Status)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoTarefaNaoExiste_DeveLancarCustomException()
        {
            // Arrange
            var tarefaId = 1;
            var request = new AtualizarTarefaRequest
            {
                Id = tarefaId,
                Titulo = "Novo Título",
                Descricao = "Nova Descrição",
                DataCriacao = DateTime.Now,
                DataConclusao = DateTime.Now.AddDays(1),
                Status = EStatusTarefa.Concluido
            };

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId)).ReturnsAsync((Tarefa)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, CancellationToken.None));
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            _tarefaRepositoryMock.Verify(repo => repo.GetByIdAsync(tarefaId), Times.Once);
            _tarefaRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Tarefa>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoOcorreErro_DeveLancarExcecao()
        {
            // Arrange
            var tarefaId = 1;
            var tarefa = new Tarefa("Título Original", "Descrição Original", DateTime.Now, null, EStatusTarefa.Pendente)
            {
                Id = tarefaId
            };

            var request = new AtualizarTarefaRequest
            {
                Id = tarefaId,
                Titulo = "Novo Título",
                Descricao = "Nova Descrição",
                DataCriacao = DateTime.Now,
                DataConclusao = DateTime.Now.AddDays(1),
                Status = EStatusTarefa.Concluido
            };

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId)).ReturnsAsync(tarefa);
            _tarefaRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Tarefa>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}