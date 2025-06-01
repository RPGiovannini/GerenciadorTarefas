using System;
using FluentValidation.TestHelper;
using GerenciadorTarefas.Application.Tarefas.Requests.CriarTarefa;
using GerenciadorTarefas.Domain.Enums;
using Xunit;

namespace GerenciadorTarefas.Tests.Application
{
    public class CriarTarefaRequestValidatorTests
    {
        private readonly CriarTarefaRequestValidator _validator;

        public CriarTarefaRequestValidatorTests()
        {
            _validator = new CriarTarefaRequestValidator();
        }

        [Fact]
        public void Validate_QuandoTituloVazio_DeveRetornarErro()
        {
            // Arrange
            var request = new CriarTarefaRequest
            {
                Titulo = string.Empty,
                DataInicio = DateTime.Now,
                Status = EStatusTarefa.Pendente
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Titulo);
        }

        [Fact]
        public void Validate_QuandoTituloExcedeLimite_DeveRetornarErro()
        {
            // Arrange
            var request = new CriarTarefaRequest
            {
                Titulo = new string('A', 101), // 101 caracteres
                DataInicio = DateTime.Now,
                Status = EStatusTarefa.Pendente
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Titulo);
        }

        [Fact]
        public void Validate_QuandoTituloValido_NaoDeveRetornarErro()
        {
            // Arrange
            var request = new CriarTarefaRequest
            {
                Titulo = "Título Válido",
                DataInicio = DateTime.Now,
                Status = EStatusTarefa.Pendente
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Titulo);
        }

        [Fact]
        public void Validate_QuandoDataInicioVazia_DeveRetornarErro()
        {
            // Arrange
            var request = new CriarTarefaRequest
            {
                Titulo = "Título Válido",
                DataInicio = default,
                Status = EStatusTarefa.Pendente
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.DataInicio);
        }

        [Fact]
        public void Validate_QuandoDataFimMenorQueDataInicio_DeveRetornarErro()
        {
            // Arrange
            var dataInicio = DateTime.Now;
            var request = new CriarTarefaRequest
            {
                Titulo = "Título Válido",
                DataInicio = dataInicio,
                DataFim = dataInicio.AddDays(-1), // Data anterior à data de início
                Status = EStatusTarefa.Pendente
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.DataFim);
        }

        [Fact]
        public void Validate_QuandoDataFimMaiorQueDataInicio_NaoDeveRetornarErro()
        {
            // Arrange
            var dataInicio = DateTime.Now;
            var request = new CriarTarefaRequest
            {
                Titulo = "Título Válido",
                DataInicio = dataInicio,
                DataFim = dataInicio.AddDays(1), // Data posterior à data de início
                Status = EStatusTarefa.Pendente
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.DataFim);
        }

        [Fact]
        public void Validate_QuandoStatusVazio_DeveRetornarErro()
        {
            // Arrange
            var request = new CriarTarefaRequest
            {
                Titulo = "Título Válido",
                DataInicio = DateTime.Now,
                Status = 0 // Valor inválido
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }
    }
}