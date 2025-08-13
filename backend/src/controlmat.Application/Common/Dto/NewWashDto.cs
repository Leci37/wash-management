using System.Collections.Generic;

namespace Controlmat.Application.Common.Dto;

public class NewWashDto
{
    public short MachineId { get; set; }
    public int StartUserId { get; set; }
    public string? StartObservation { get; set; }
    public List<ProtDto> ProtEntries { get; set; } = new();
}
