// 📁 controlmat.Application.Common.Dto
// This file contains all the Data Transfer Objects (DTOs) used in the SUMISAN Wash Management System backend.
// Each class represents a request or response model used in API interactions.
// These are read and processed by MediatR-based Command/Query handlers as defined by the Gestraf architecture.
// 
// ⚠️ NOTE: Authentication is handled via Keycloak OIDC. No login DTOs are included here.
// User identity comes from JWT claims in HttpContext.User (preferred_username, sub, name, roles).

namespace Controlmat.Application.Common.Dto
{
    // 🚿 NewWashDto
    // Sent when starting a new wash cycle. Contains machine, operator, optional observations, and at least one Prot entry.
    public class NewWashDto
    {
        public int MachineId { get; set; }                    // Must be 1 or 2
        public int StartUserId { get; set; }                  // Operator initiating the wash
        public string? StartObservation { get; set; }         // Optional field for initial notes
        public List<ProtDto> ProtEntries { get; set; } = new(); // Required: at least one Prot
    }

    // 🔬 ProtDto
    // Represents a single instrument set (Prot) scanned during the wash. Required for NewWash and AddProt operations.
    public class ProtDto
    {
        public string ProtId { get; set; } = string.Empty;       // Format: PROTXXX (e.g., PROT001)
        public string BatchNumber { get; set; } = string.Empty;  // Format: NLXX (e.g., NL01)
        public string BagNumber { get; set; } = string.Empty;    // Format: XX/XX (e.g., 01/02)
    }

    // 🧺 FinishWashDto
    // Used when finalizing a wash. Includes the operator and optional end-of-process notes.
    public class FinishWashDto
    {
        public int EndUserId { get; set; }                       // Operator finishing the wash
        public string? FinishObservation { get; set; }           // Optional field for final notes
    }

    // 📦 AddProtDto
    // Enables adding a Prot to an ongoing wash. Used when scanning more instrument sets after starting the wash.
    public class AddProtDto
    {
        public long WashingId { get; set; }                      // ID of the target washing cycle
        public string ProtId { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string BagNumber { get; set; } = string.Empty;
    }

    // 🖼️ PhotoUploadDto
    // Optional metadata holder if image uploads are handled manually via form-data + API route.
    public class PhotoUploadDto
    {
        public IFormFile File { get; set; } = default!;         // The uploaded file (JPEG/PNG expected)
        public string? Description { get; set; }                // Optional comment for the image
    }

    // 📷 PhotoDto
    // Returned when querying or listing photos related to a wash. Used in summary views.
    public class PhotoDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;    // e.g., 25080601_01.jpg
        public string FilePath { get; set; } = string.Empty;    // Full/relative file system path
        public DateTime CreatedAt { get; set; }                 // Timestamp of upload
    }

    // 🔍 WashingResponseDto
    // Composite DTO returned when retrieving details of a wash (e.g., for GET by ID or summary).
    public class WashingResponseDto
    {
        public long WashingId { get; set; }
        public int MachineId { get; set; }
        public int StartUserId { get; set; }
        public int? EndUserId { get; set; }                     // Nullable until wash is finalized
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;      // 'P' (In Progress), 'F' (Finished)
        public string? StartObservation { get; set; }
        public string? FinishObservation { get; set; }
        public List<ProtDto> Prots { get; set; } = new();       // All scanned prot entries
        public List<PhotoDto> Photos { get; set; } = new();     // Uploaded evidence
    }

    // 👤 UserDto
    // Simple user representation for dropdowns and references
    // Populated from database Users table, not from JWT claims
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    // 🏭 MachineDto  
    // Machine representation for dropdowns and availability checking
    public class MachineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;           // Calculated based on active washes
    }
}