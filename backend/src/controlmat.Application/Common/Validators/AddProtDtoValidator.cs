using FluentValidation;
using controlmat.Application.Common.Dto;

namespace controlmat.Application.Common.Validators;

public class AddProtDtoValidator : AbstractValidator<AddProtDto>
{
    public AddProtDtoValidator()
    {
        RuleFor(x => x.WashingId)
            .NotEmpty().WithMessage("WashingId is required")
            .GreaterThan(0).WithMessage("WashingId must be a positive number");

        RuleFor(x => x.ProtId)
            .NotEmpty().WithMessage("ProtId is required")
            .Matches(@"^PROT[0-9]{3}$").WithMessage("ProtId must follow format PROTXXX (e.g., PROT001)");

        RuleFor(x => x.BatchNumber)
            .NotEmpty().WithMessage("BatchNumber is required")
            .Matches(@"^NL[0-9]{2}$").WithMessage("BatchNumber must follow format NLXX (e.g., NL01)");

        RuleFor(x => x.BagNumber)
            .NotEmpty().WithMessage("BagNumber is required")
            .Matches(@"^[0-9]{2}/[0-9]{2}$").WithMessage("BagNumber must follow format XX/XX (e.g., 01/02)");
    }
}
