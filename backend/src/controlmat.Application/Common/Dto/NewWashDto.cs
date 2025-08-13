using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Controlmat.Application.Common.Dto;

public class NewWashDto
{
    [Required]
    [Range(1, 4)]
    public short MachineId { get; set; } = 1;

    [Required]
    [Range(1, int.MaxValue)]
    public int StartUserId { get; set; } = 1;

    public string? StartObservation { get; set; }
    public List<ProtDto> ProtEntries { get; set; } = new();
}
