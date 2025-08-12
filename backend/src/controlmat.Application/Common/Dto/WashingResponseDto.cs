namespace controlmat.Application.Common.Dto;

public class WashingResponseDto
{
    public long WashingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ProtDto> Prots { get; set; } = new();
    public List<PhotoDto> Photos { get; set; } = new();
}
