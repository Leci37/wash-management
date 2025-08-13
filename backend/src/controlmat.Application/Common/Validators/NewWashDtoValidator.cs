using FluentValidation;
using Controlmat.Application.Common.Dto;

namespace Controlmat.Application.Common.Validators;

public class NewWashDtoValidator : AbstractValidator<NewWashDto>
{
    public NewWashDtoValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty().WithMessage("MachineId is required")
            .InclusiveBetween((short)1, (short)4)
            .WithMessage("MachineId must be between 1 and 4");

        RuleFor(x => x.StartUserId)
            .NotEmpty().WithMessage("StartUserId is required")
            .GreaterThan(0).WithMessage("StartUserId must be a positive number");

        RuleFor(x => x.ProtEntries)
            .NotEmpty().WithMessage("At least one PROT entry is required")
            .Must(prots => prots != null && prots.Count >= 1)
            .WithMessage("At least one PROT entry is required");

        RuleFor(x => x.StartObservation)
            .MaximumLength(100).WithMessage("StartObservation cannot exceed 100 characters");

        RuleForEach(x => x.ProtEntries)
            .SetValidator(new ProtDtoValidator());
    }
}
