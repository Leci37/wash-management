
using System;


namespace Controlmat.Application.Common.Dto;

public class ActiveWashDto
{
    public int MachineId { get; set; }
    public long WashingId { get; set; }
    public DateTime StartDate { get; set; }
    public string StartUserName { get; set; } = string.Empty;

}
