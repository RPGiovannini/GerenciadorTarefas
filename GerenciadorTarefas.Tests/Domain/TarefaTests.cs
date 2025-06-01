using System;
using Xunit;
using FluentAssertions;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Enums;

namespace GerenciadorTarefas.Tests.Domain
{
    public class TarefaTests
    {
        [Fact]
        public void CriarTarefa_DeveInicializarCorretamente()
        {
            // Arrange
            string titulo = "Tarefa de Teste";
            string descricao = "Descrição de teste";
            DateTime dataCriacao = DateTime.Now;
            DateTime? dataConclusao = null;
            EStatusTarefa status = EStatusTarefa.Pendente;

            // Act
            var tarefa = new Tarefa(titulo, descricao, dataCriacao, dataConclusao, status);

            // Assert
            tarefa.Titulo.Should().Be(titulo);
            tarefa.Descricao.Should().Be(descricao);
            tarefa.DataCriacao.Should().Be(dataCriacao);
            tarefa.DataConclusao.Should().BeNull();
            tarefa.Status.Should().Be(EStatusTarefa.Pendente);
        }

        [Fact]
        public void AtualizarTarefa_DeveAtualizarPropriedadesCorretamente()
        {
            // Arrange
            var tarefa = new Tarefa("Título Antigo", "Descrição Antiga", DateTime.Now, null, EStatusTarefa.Pendente);
            string novoTitulo = "Título Novo";
            string novaDescricao = "Nova Descrição";
            DateTime? novaDataConclusao = DateTime.Now.AddDays(1);
            EStatusTarefa novoStatus = EStatusTarefa.EmAndamento;

            // Act
            tarefa.AtualizarTarefa(novoTitulo, novaDescricao, novaDataConclusao, novoStatus);

            // Assert
            tarefa.Titulo.Should().Be(novoTitulo);
            tarefa.Descricao.Should().Be(novaDescricao);
            tarefa.DataConclusao.Should().Be(novaDataConclusao);
            tarefa.Status.Should().Be(novoStatus);
        }

        [Fact]
        public void ConcluirTarefa_QuandoStatusPendente_DeveAlterarParaConcluido()
        {
            // Arrange
            var tarefa = new Tarefa("Teste", "Descrição", DateTime.Now, null, EStatusTarefa.Pendente);

            // Act
            tarefa.ConcluirTarefa();

            // Assert
            tarefa.Status.Should().Be(EStatusTarefa.Concluido);
        }

        [Fact]
        public void ConcluirTarefa_QuandoStatusNaoPendente_NaoDeveAlterarStatus()
        {
            // Arrange
            var tarefa = new Tarefa("Teste", "Descrição", DateTime.Now, null, EStatusTarefa.EmAndamento);

            // Act
            tarefa.ConcluirTarefa();

            // Assert
            tarefa.Status.Should().Be(EStatusTarefa.EmAndamento);
        }
    }
}