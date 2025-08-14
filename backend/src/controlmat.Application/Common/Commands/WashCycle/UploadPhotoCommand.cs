using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Controlmat.Domain.Interfaces;
using Controlmat.Domain.Entities;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Exceptions;

namespace Controlmat.Application.Common.Commands.WashCycle;

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
                var description = request.Description;

                if (file == null)
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - NO FILE PROVIDED. WashingId: {WashingId}", function, threadId, washingId);
                    throw new ArgumentException(ValidationErrorMessages.Photo.FileRequired);
                }

                if (file.Length == 0)
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - EMPTY FILE. WashingId: {WashingId}", function, threadId, washingId);
                    throw new ArgumentException(ValidationErrorMessages.Photo.FileEmpty);
                }

                if (!string.IsNullOrEmpty(description) && description.Length > 200)
                    throw new ArgumentException(ValidationErrorMessages.Photo.DescriptionTooLong(200));

                if (!IsValidWashingId(washingId))
                    throw new ArgumentException(ValidationErrorMessages.Washing.InvalidIdFormat(washingId));

                var washing = await _washingRepo.GetByIdAsync(washingId);
                if (washing == null)
                    throw new InvalidOperationException(ValidationErrorMessages.Washing.NotFound(washingId));

                if (washing.Status != 'P')
                {
                    _logger.LogWarning("⚠️ Cannot modify washing with status '{Status}': {WashingId}",
                        washing.Status, washingId);
                    throw new ConflictException(ValidationErrorMessages.Washing.CannotModifyFinished(washingId));
                }

                var currentPhotoCount = await _photoRepo.CountByWashingIdAsync(washingId);
                if (currentPhotoCount >= MaxPhotosPerWash)
                    throw new InvalidOperationException(ValidationErrorMessages.Photo.MaxPhotosReached(washingId, MaxPhotosPerWash));

                var contentType = file.ContentType?.ToLower() ?? string.Empty;
                if (!AllowedContentTypes.Contains(contentType))
                    throw new ArgumentException(ValidationErrorMessages.Photo.InvalidFileType(contentType));

                if (file.Length > MaxFileSizeBytes)
                    throw new ArgumentException(ValidationErrorMessages.Photo.FileSizeExceeded(file.Length, 5));

                if (!IsValidImage(file))
                    throw new ArgumentException(ValidationErrorMessages.Photo.InvalidImageContent);

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
            catch (ConflictException ex)
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

        private static bool IsValidWashingId(long washingId)
        {
            var idStr = washingId.ToString();
            if (!Regex.IsMatch(idStr, @"^\d{8}$"))
                return false;
            return DateTime.TryParseExact(idStr.Substring(0, 6), "yyMMdd", null, System.Globalization.DateTimeStyles.None, out _);
        }

        private static bool IsValidImage(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                Span<byte> header = stackalloc byte[8];
                if (stream.Read(header) < 8)
                    return false;

                // JPEG signature FF D8 FF
                if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                    return true;

                // PNG signature 89 50 4E 47 0D 0A 1A 0A
                if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                    header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
