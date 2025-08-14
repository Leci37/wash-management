using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Application.Common.Constants;
using Controlmat.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Controlmat.Application.Common.Queries.Photo;

public static class DownloadSinglePhotoQuery
{
    public record Request(int PhotoId) : IRequest<PhotoDownloadDto>;

    public class Handler : IRequestHandler<Request, PhotoDownloadDto>
    {
        private readonly IPhotoRepository _repository;
        private readonly ILogger<Handler> _logger;

        public Handler(IPhotoRepository repository, ILogger<Handler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PhotoDownloadDto> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 DownloadSinglePhotoQuery - STARTED. PhotoId: {PhotoId}", request.PhotoId);

            if (!await _repository.ExistsAsync(request.PhotoId))
            {
                _logger.LogWarning("⚠️ Photo not found: {PhotoId}", request.PhotoId);
                throw new ValidationException(ValidationErrorMessages.Photo.NotFound(request.PhotoId));
            }

            var photo = await _repository.GetByIdAsync(request.PhotoId);
            if (photo == null)
            {
                _logger.LogWarning("⚠️ Photo not found after existence check: {PhotoId}", request.PhotoId);
                throw new ValidationException(ValidationErrorMessages.Photo.NotFound(request.PhotoId));
            }

            if (!File.Exists(photo.FilePath))
            {
                _logger.LogError("❌ Photo file not found on disk: {FilePath}", photo.FilePath);
                throw new ValidationException(ValidationErrorMessages.Photo.FileNotFound(photo.FileName));
            }

            byte[] fileBytes;
            try
            {
                fileBytes = await File.ReadAllBytesAsync(photo.FilePath, ct);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("❌ Access denied to photo file: {FilePath}", photo.FilePath);
                throw new ValidationException(ValidationErrorMessages.Photo.FileAccessDenied(photo.FileName));
            }

            var contentType = Path.GetExtension(photo.FileName).ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            _logger.LogInformation("✅ DownloadSinglePhotoQuery - COMPLETED. File size: {Size} bytes", fileBytes.Length);

            return new PhotoDownloadDto
            {
                FileBytes = fileBytes,
                ContentType = contentType,
                FileName = photo.FileName
            };
        }
    }
}
