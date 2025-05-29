using FluentValidation;

namespace GerenciadorTarefas.Application.Tarefas.Requests.CriarTarefa
{
    public class CriarTarefaRequestValidator : AbstractValidator<CriarTarefaRequest>
    {
        public CriarTarefaRequestValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título é obrigatório")
                .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres");

            RuleFor(x => x.DataInicio)
                .NotEmpty().WithMessage("A data de início é obrigatória");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("O status é obrigatório");

            RuleFor(x => x.DataFim)
                .GreaterThan(x => x.DataInicio).WithMessage("A data de fim deve ser maior que a data de início");
        }
    }
}

