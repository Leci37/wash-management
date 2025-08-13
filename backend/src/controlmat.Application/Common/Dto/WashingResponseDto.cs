
using System;
using System.Collections.Generic;

namespace Controlmat.Application.Common.Dto;

public class WashingResponseDto
{
    public long WashingId { get; set; }

    public short MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public int StartUserId { get; set; }
    public string StartUserName { get; set; } = string.Empty;
    public int? EndUserId { get; set; }
    public string? EndUserName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public char Status { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
    public string? StartObservation { get; set; }
    public string? FinishObservation { get; set; }
    public List<ProtDto> Prots { get; set; } = new();
    public List<PhotoDto> Photos { get; set; } = new();
}
