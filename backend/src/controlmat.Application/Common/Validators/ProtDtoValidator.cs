using FluentValidation;
using Controlmat.Application.Common.Dto;

namespace Controlmat.Application.Common.Validators;

public class ProtDtoValidator : AbstractValidator<ProtDto>
{
    public ProtDtoValidator()
    {
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
