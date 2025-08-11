namespace Controlmat.Application.Common.Dto
{
    public class NewWashDto
    {
        public int MachineId { get; set; }
        public int StartUserId { get; set; }
        public string? StartObservation { get; set; }
        public List<ProtDto> ProtEntries { get; set; } = new();
    }
}
