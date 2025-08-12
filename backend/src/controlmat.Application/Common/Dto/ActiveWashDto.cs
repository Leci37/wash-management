namespace controlmat.Application.Common.Dto;

public class ActiveWashDto
{
    public long WashingId { get; set; }
    public short MachineId { get; set; }
    public int StartUserId { get; set; }
    public DateTime StartDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
