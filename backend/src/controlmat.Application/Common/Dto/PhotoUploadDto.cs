using Microsoft.AspNetCore.Http;

namespace Controlmat.Application.Common.Dto;

public class PhotoUploadDto
{
    public IFormFile File { get; set; } = default!;
    public string? Description { get; set; }
}
