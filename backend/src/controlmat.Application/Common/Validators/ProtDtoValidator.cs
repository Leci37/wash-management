using FluentValidation;
using Controlmat.Application.Common.Dto;

namespace Controlmat.Application.Common.Validators
{
    public class ProtDtoValidator : AbstractValidator<ProtDto>
    {
        public ProtDtoValidator()
        {
            RuleFor(x => x.ProtId)
                .NotEmpty()
                .Matches(@"^PROT[0-9]{3}$")
                .WithMessage("ProtId must follow format PROTXXX");

            RuleFor(x => x.BatchNumber)
                .NotEmpty()
                .Matches(@"^NL[0-9]{2}$")
                .WithMessage("BatchNumber must follow format NLXX");

            RuleFor(x => x.BagNumber)
                .NotEmpty()
                .Matches(@"^[0-9]{2}/[0-9]{2}$")
                .WithMessage("BagNumber must follow format XX/XX");
        }
    }
}
