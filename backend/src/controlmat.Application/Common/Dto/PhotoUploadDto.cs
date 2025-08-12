using Microsoft.AspNetCore.Http;

namespace controlmat.Application.Common.Dto;

public class PhotoUploadDto
{
    public IFormFile File { get; set; } = default!;
    public string? Description { get; set; }
}
