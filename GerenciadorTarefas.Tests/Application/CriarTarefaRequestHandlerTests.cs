using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GerenciadorTarefas.Application.Tarefas.Requests.CriarTarefa;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Enums;
using GerenciadorTarefas.Domain.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GerenciadorTarefas.Tests.Application
{
    public class CriarTarefaRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITarefaRepository> _tarefaRepositoryMock;
        private readonly Mock<ILogger<CriarTarefaResquestHandler>> _loggerMock;
        private readonly CriarTarefaResquestHandler _handler;

        public CriarTarefaRequestHandlerTests()
        {
            _tarefaRepositoryMock = new Mock<ITarefaRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CriarTarefaResquestHandler>>();

            _unitOfWorkMock.Setup(uow => uow.TarefaRepository).Returns(_tarefaRepositoryMock.Object);
            _unitOfWorkMock.Setup(uow => uow.Commit()).ReturnsAsync(true);

            _handler = new CriarTarefaResquestHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_DeveCriarTarefaERetornarId()
        {
            // Arrange
            var request = new CriarTarefaRequest
            {
                Titulo = "Tarefa de Teste",
                Descricao = "Descrição de teste",
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now.AddDays(1),
                Status = EStatusTarefa.Pendente
            };

            _tarefaRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Tarefa>()))
                .Callback<Tarefa>(tarefa => tarefa.Id = 1)
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _tarefaRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Tarefa>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoOcorreErro_DeveLancarExcecao()
        {
            // Arrange
            var request = new CriarTarefaRequest
            {
                Titulo = "Tarefa de Teste",
                Descricao = "Descrição de teste",
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now.AddDays(1),
                Status = EStatusTarefa.Pendente
            };

            _tarefaRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Tarefa>()))
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