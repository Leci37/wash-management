using System;

namespace Controlmat.Application.Common.Dto;

public class PhotoDownloadDto
{
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
