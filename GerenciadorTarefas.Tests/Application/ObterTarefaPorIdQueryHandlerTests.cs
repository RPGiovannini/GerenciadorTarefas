using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GerenciadorTarefas.Application.Tarefas.Querys;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Enums;
using GerenciadorTarefas.Domain.Exceptions;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GerenciadorTarefas.Tests.Application
{
    public class ObterTarefaPorIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITarefaRepository> _tarefaRepositoryMock;
        private readonly Mock<ILogger<ObterTarefaPorIdQueryHandler>> _loggerMock;
        private readonly ObterTarefaPorIdQueryHandler _handler;

        public ObterTarefaPorIdQueryHandlerTests()
        {
            _tarefaRepositoryMock = new Mock<ITarefaRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ObterTarefaPorIdQueryHandler>>();

            _unitOfWorkMock.Setup(uow => uow.TarefaRepository).Returns(_tarefaRepositoryMock.Object);

            _handler = new ObterTarefaPorIdQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_QuandoTarefaExiste_DeveRetornarTarefa()
        {
            // Arrange
            var tarefaId = 1;
            var tarefa = new Tarefa("Tarefa 1", "Descrição 1", DateTime.Now, null, EStatusTarefa.Pendente) { Id = tarefaId };
            var query = new ObterTarefaPorIdQuery(tarefaId);

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId)).ReturnsAsync(tarefa);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(tarefa);
            _tarefaRepositoryMock.Verify(repo => repo.GetByIdAsync(tarefaId), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoTarefaNaoExiste_DeveLancarCustomException()
        {
            // Arrange
            var tarefaId = 1;
            var query = new ObterTarefaPorIdQuery(tarefaId);

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId)).ReturnsAsync((Tarefa)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(query, CancellationToken.None));
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Message.Should().Contain($"Tarefa com ID {tarefaId} não foi encontrada");

            _tarefaRepositoryMock.Verify(repo => repo.GetByIdAsync(tarefaId), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Tarefa com ID {tarefaId} não foi encontrada")),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoOcorreErro_DeveLancarExcecao()
        {
            // Arrange
            var tarefaId = 1;
            var query = new ObterTarefaPorIdQuery(tarefaId);

            _tarefaRepositoryMock.Setup(repo => repo.GetByIdAsync(tarefaId))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
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