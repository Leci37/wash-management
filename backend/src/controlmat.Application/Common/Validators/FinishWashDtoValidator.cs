using FluentValidation;
using controlmat.Application.Common.Dto;

namespace controlmat.Application.Common.Validators;

public class FinishWashDtoValidator : AbstractValidator<FinishWashDto>
{
    public FinishWashDtoValidator()
    {
        RuleFor(x => x.EndUserId)
            .NotEmpty().WithMessage("EndUserId is required")
            .GreaterThan(0).WithMessage("EndUserId must be a positive number");

        RuleFor(x => x.FinishObservation)
            .MaximumLength(100).WithMessage("FinishObservation cannot exceed 100 characters");
    }
}
