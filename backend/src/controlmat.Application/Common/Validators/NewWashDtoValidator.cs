using FluentValidation;
using controlmat.Application.Common.Dto;

namespace controlmat.Application.Common.Validators;

public class NewWashDtoValidator : AbstractValidator<NewWashDto>
{
    public NewWashDtoValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty().WithMessage("MachineId is required")
            .InclusiveBetween(1, 2).WithMessage("MachineId must be 1 or 2");

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
