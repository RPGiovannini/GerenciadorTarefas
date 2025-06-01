using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GerenciadorTarefas.Application.Tarefas.Requests.DeletarTarefa;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Enums;
using GerenciadorTarefas.Domain.Exceptions;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GerenciadorTarefas.Tests.Application
{
    public class DeletarTarefaRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITarefaRepository> _tarefaRepositoryMock;
        private readonly Mock<ILogger<DeletarTarefaRequestHandler>> _loggerMock;
        private readonly DeletarTarefaRequestHandler _handler;

        public DeletarTarefaRequestHandlerTests()
        {
            _tarefaRepositoryMock = new Mock<ITarefaRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeletarTarefaRequestHandler>>();

            _unitOfWorkMock.Setup(uow => uow.TarefaRepository).Returns(_tarefaRepositoryMock.Object);
            _unitOfWorkMock.Setup(uow => uow.Commit()).ReturnsAsync(true);

            _handler = new DeletarTarefaRequestHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_QuandoTarefaExiste_DeveDeletarTarefaERetornarTrue()
        {
            // Arrange
            var tarefaId = 1;
            var tarefa = new Tarefa("Título", "Descrição", DateTime.Now, null, EStatusTarefa.Pendente)
            {
                Id = tarefaId
            };

            var request = new DeletarTarefaRequest
            {
                Id = tarefaId
            };

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId)).ReturnsAsync(tarefa);
            _tarefaRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Tarefa>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _tarefaRepositoryMock.Verify(repo => repo.GetByIdAsync(tarefaId), Times.Once);
            _tarefaRepositoryMock.Verify(repo => repo.DeleteAsync(tarefa), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoTarefaNaoExiste_DeveLancarCustomException()
        {
            // Arrange
            var tarefaId = 1;
            var request = new DeletarTarefaRequest
            {
                Id = tarefaId
            };

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId)).ReturnsAsync((Tarefa)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, CancellationToken.None));
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            _tarefaRepositoryMock.Verify(repo => repo.GetByIdAsync(tarefaId), Times.Once);
            _tarefaRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Tarefa>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoOcorreErro_DeveLancarExcecao()
        {
            // Arrange
            var tarefaId = 1;
            var tarefa = new Tarefa("Título", "Descrição", DateTime.Now, null, EStatusTarefa.Pendente)
            {
                Id = tarefaId
            };

            var request = new DeletarTarefaRequest
            {
                Id = tarefaId
            };

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId)).ReturnsAsync(tarefa);
            _tarefaRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Tarefa>()))
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