using FluentValidation;

namespace GerenciadorTarefas.Application.Tarefas.Requests.AtualizarTarefa
{
    public class AtualizarTarefaRequestValidator : AbstractValidator<AtualizarTarefaRequest>
    {
        public AtualizarTarefaRequestValidator()
        {
            RuleFor(x => x.Titulo)
                 .NotEmpty().WithMessage("O título é obrigatório")
                 .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres");

            RuleFor(x => x.DataCriacao)
                .NotEmpty().WithMessage("A data de início é obrigatória");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("O status é obrigatório");

            RuleFor(x => x.DataConclusao)
                .GreaterThan(x => x.DataCriacao).WithMessage("A data de conclusão deve ser maior que a data de início");


        }
    }
}
