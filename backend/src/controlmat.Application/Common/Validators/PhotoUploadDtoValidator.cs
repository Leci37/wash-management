using System.Linq;
using FluentValidation;
using Controlmat.Application.Common.Dto;

namespace Controlmat.Application.Common.Validators;

public class PhotoUploadDtoValidator : AbstractValidator<PhotoUploadDto>
{
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/jpg", "image/png" };

    public PhotoUploadDtoValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required");

        RuleFor(x => x.File.ContentType)
            .Must(contentType => AllowedContentTypes.Contains(contentType?.ToLower()))
            .WithMessage("File must be JPEG or PNG format")
            .When(x => x.File != null);

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(5 * 1024 * 1024)
            .WithMessage("File size cannot exceed 5MB")
            .When(x => x.File != null);

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");
    }
}
