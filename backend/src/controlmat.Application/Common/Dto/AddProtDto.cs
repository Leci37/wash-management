namespace controlmat.Application.Common.Dto;

public class AddProtDto
{
    public long WashingId { get; set; }
    public string ProtId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string BagNumber { get; set; } = string.Empty;
}
