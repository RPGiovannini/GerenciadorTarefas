using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GerenciadorTarefas.Application.Tarefas.Querys;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Enums;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GerenciadorTarefas.Tests.Application
{
    public class ObterTarefaQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITarefaRepository> _tarefaRepositoryMock;
        private readonly Mock<ILogger<ObterTarefaQueryHandler>> _loggerMock;
        private readonly ObterTarefaQueryHandler _handler;

        public ObterTarefaQueryHandlerTests()
        {
            _tarefaRepositoryMock = new Mock<ITarefaRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ObterTarefaQueryHandler>>();

            _unitOfWorkMock.Setup(uow => uow.TarefaRepository).Returns(_tarefaRepositoryMock.Object);

            _handler = new ObterTarefaQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_DeveRetornarListaDeTarefas()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                new Tarefa("Tarefa 1", "Descrição 1", DateTime.Now, null, EStatusTarefa.Pendente) { Id = 1 },
                new Tarefa("Tarefa 2", "Descrição 2", DateTime.Now, null, EStatusTarefa.EmAndamento) { Id = 2 }
            };

            _tarefaRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tarefas);

            // Act
            var result = await _handler.Handle(new ObterTarefaQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(tarefas);
            _tarefaRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var tarefas = new List<Tarefa>();

            _tarefaRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tarefas);

            // Act
            var result = await _handler.Handle(new ObterTarefaQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
            _tarefaRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoOcorreErro_DeveLancarExcecao()
        {
            // Arrange
            _tarefaRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new ObterTarefaQuery(), CancellationToken.None));
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