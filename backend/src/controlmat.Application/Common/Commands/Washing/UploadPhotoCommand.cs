using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using controlmat.Domain.Interfaces;
using controlmat.Domain.Entities;
using System.Security.Claims;

namespace controlmat.Application.Common.Commands.Washing;

public static class UploadPhotoCommand
{
    public class Request : IRequest<string>
    {
        public long WashingId { get; set; }
        public IFormFile File { get; set; } = default!;
        public string? Description { get; set; }
    }

    public class Handler : IRequestHandler<Request, string>
    {
        private readonly IWashingRepository _washingRepo;
        private readonly IPhotoRepository _photoRepo;
        private readonly IParameterRepository _paramRepo;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger<Handler> _logger;

        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/jpg", "image/png" };
        private const int MaxFileSizeBytes = 5 * 1024 * 1024;
        private const int MaxPhotosPerWash = 99;

        public Handler(
            IWashingRepository washingRepo,
            IPhotoRepository photoRepo,
            IParameterRepository paramRepo,
            IConfiguration config,
            IHttpContextAccessor httpContext,
            ILogger<Handler> logger)
        {
            _washingRepo = washingRepo;
            _photoRepo = photoRepo;
            _paramRepo = paramRepo;
            _config = config;
            _httpContext = httpContext;
            _logger = logger;
        }

        public async Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            var function = nameof(Handle);
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var washingId = request.WashingId;
            var file = request.File;

            _logger.LogInformation("🌀 {Function} [Thread:{ThreadId}] - STARTED. WashingId: {WashingId}, FileName: {FileName}, Size: {Size} bytes",
                function, threadId, washingId, file?.FileName, file?.Length ?? 0);

            try
            {
                var currentUser = GetCurrentUserFromClaims();

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - NO FILE PROVIDED. WashingId: {WashingId}",
                        function, threadId, washingId);
                    throw new ArgumentException("File is required");
                }

                var washing = await _washingRepo.GetByIdAsync(washingId);
                if (washing == null)
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - WASHING NOT FOUND. WashingId: {WashingId}",
                        function, threadId, washingId);
                    throw new InvalidOperationException($"Washing with ID {washingId} not found");
                }

                if (washing.Status != "P")
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - WASHING NOT IN PROGRESS. WashingId: {WashingId}, Status: {Status}",
                        function, threadId, washingId, washing.Status);
                    throw new InvalidOperationException($"Cannot upload photos to finished wash {washingId}");
                }

                var currentPhotoCount = await _photoRepo.CountByWashingIdAsync(washingId);
                if (currentPhotoCount >= MaxPhotosPerWash)
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - PHOTO LIMIT EXCEEDED. WashingId: {WashingId}, Current: {Count}",
                        function, threadId, washingId, currentPhotoCount);
                    throw new InvalidOperationException($"Maximum {MaxPhotosPerWash} photos allowed per wash");
                }

                if (!AllowedContentTypes.Contains(file.ContentType?.ToLower()))
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - INVALID FILE TYPE. ContentType: {ContentType}",
                        function, threadId, file.ContentType);
                    throw new ArgumentException($"File type {file.ContentType} not allowed. Use JPEG or PNG");
                }

                if (file.Length > MaxFileSizeBytes)
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - FILE TOO LARGE. Size: {Size} bytes",
                        function, threadId, file.Length);
                    throw new ArgumentException($"File size exceeds {MaxFileSizeBytes / (1024 * 1024)}MB limit");
                }

                var imageBasePath = await _paramRepo.GetValueAsync("ImagePath") ?? "C:\\SumiSan\\Photos";
                var currentYear = DateTime.Now.Year.ToString();
                var yearPath = Path.Combine(imageBasePath, currentYear);
                Directory.CreateDirectory(yearPath);

                var nextSequence = currentPhotoCount + 1;
                var fileName = $"{washingId}_{nextSequence:00}.jpg";
                var fullFilePath = Path.Combine(yearPath, fileName);

                using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream, cancellationToken);
                }

                var photo = new Photo
                {
                    WashingId = washingId,
                    FileName = fileName,
                    FilePath = fullFilePath,
                    CreatedAt = DateTime.UtcNow
                };

                await _photoRepo.AddAsync(photo);

                _logger.LogInformation("✅ {Function} [Thread:{ThreadId}] - COMPLETED. WashingId: {WashingId}, FileName: {FileName}, FilePath: {FilePath}",
                    function, threadId, washingId, fileName, fullFilePath);

                return fileName;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "⚠️ {Function} [Thread:{ThreadId}] - VALIDATION ERROR. WashingId: {WashingId}",
                    function, threadId, washingId);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "⚠️ {Function} [Thread:{ThreadId}] - BUSINESS RULE VIOLATION. WashingId: {WashingId}",
                    function, threadId, washingId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ {Function} [Thread:{ThreadId}] - ERROR. WashingId: {WashingId}",
                    function, threadId, washingId);
                throw;
            }
        }

        private string? GetCurrentUserFromClaims()
        {
            var user = _httpContext.HttpContext?.User;
            return user?.FindFirst("preferred_username")?.Value
                ?? user?.FindFirst(ClaimTypes.Name)?.Value
                ?? user?.FindFirst("name")?.Value;
        }
    }
}
