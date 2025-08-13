
using System;


namespace Controlmat.Application.Common.Dto;

public class PhotoDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
