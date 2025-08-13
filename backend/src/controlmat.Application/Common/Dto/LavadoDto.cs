using System;
using System.Collections.Generic;

namespace Controlmat.Application.Common.Dto;

public class LavadoDto
{
    public long? WashingId { get; set; }
    public short MachineId { get; set; }
    public int StartUserId { get; set; }
    public int? EndUserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "P";
    public string? StartObservation { get; set; }
    public string? FinishObservation { get; set; }
    public List<ProtDto> Prots { get; set; } = new();
    public List<string> Photos { get; set; } = new();
}
