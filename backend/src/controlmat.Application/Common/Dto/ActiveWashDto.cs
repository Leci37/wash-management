using System;

namespace controlmat.Application.Common.Dto;

public class ActiveWashDto
{
    public long WashingId { get; set; }
    public int MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string StartUserName { get; set; } = string.Empty;
    public int ProtCount { get; set; }
    public int PhotoCount { get; set; }
}
