using System.ComponentModel.DataAnnotations;

namespace Controlmat.Application.Common.Dto;

public class ProtDto
{
    [Required]
    public string ProtId { get; set; } = string.Empty;

    [Required]
    public string BatchNumber { get; set; } = string.Empty;

    [Required]
    public string BagNumber { get; set; } = string.Empty;
}
