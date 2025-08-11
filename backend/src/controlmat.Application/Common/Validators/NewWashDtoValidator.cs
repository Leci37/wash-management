using FluentValidation;
using Controlmat.Application.Common.Dto;

namespace Controlmat.Application.Common.Validators
{
    public class NewWashDtoValidator : AbstractValidator<NewWashDto>
    {
        public NewWashDtoValidator()
        {
            RuleFor(x => x.MachineId)
                .NotEmpty()
                .InclusiveBetween(1, 2)
                .WithMessage("MachineId must be 1 or 2");

            RuleFor(x => x.StartUserId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("StartUserId is required");

            RuleFor(x => x.ProtEntries)
                .NotEmpty()
                .WithMessage("At least one Prot is required");

            RuleForEach(x => x.ProtEntries)
                .SetValidator(new ProtDtoValidator());

            RuleFor(x => x.StartObservation)
                .MaximumLength(100)
                .WithMessage("StartObservation cannot exceed 100 characters");
        }
    }
}
