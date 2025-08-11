using Controlmat.Domain.Enums;

namespace Controlmat.Domain.Entities;

public class Washing
{
    // Aggregate Root
    public string WashingId { get; private set; } = null!; // format YYMMDDXX (generated outside Domain)
    public int MachineId { get; private set; }
    public int StartUserId { get; private set; }
    public int? EndUserId { get; private set; }

    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    public string? StartObservation { get; private set; }
    public string? FinishObservation { get; private set; }

    public WashingStatus Status { get; private set; } = WashingStatus.InProgress;

    private readonly List<Prot> _prots = new();
    public IReadOnlyCollection<Prot> Prots => _prots;

    private readonly List<Photo> _photos = new();
    public IReadOnlyCollection<Photo> Photos => _photos;

    // Minimal behavior that's safe for Domain (no cross-aggregate logic)
    public void AddProt(Prot prot) => _prots.Add(prot);
    public void AddPhoto(Photo photo) => _photos.Add(photo);

    public void MarkFinished(int endUserId, DateTime endedAt, string? observation = null)
    {
        EndUserId = endUserId;
        EndedAt = endedAt;
        FinishObservation = observation;
        Status = WashingStatus.Finished;
    }

    // Factory or constructor is OK; handlers will enforce business rules first.
    public static Washing Create(string washingId, int machineId, int startUserId, DateTime startedAt, string? obs = null)
        => new Washing
        {
            WashingId = washingId,
            MachineId = machineId,
            StartUserId = startUserId,
            StartedAt = startedAt,
            StartObservation = obs,
            Status = WashingStatus.InProgress
        };
}
