using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Exceptions;
using Controlmat.Domain.Interfaces;
using System.IO;

namespace Controlmat.Application.Common.Queries.Photo;

public static class DownloadSinglePhotoQuery
{
    public record Request(int PhotoId) : IRequest<PhotoDownloadDto>;

    public class Handler : IRequestHandler<Request, PhotoDownloadDto>
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IParameterRepository _parameterRepository;
        private readonly ILogger<Handler> _logger;
        private string _imagePath = "C\\SumiSan\\Photos";

        public Handler(IPhotoRepository photoRepository, IParameterRepository parameterRepository, ILogger<Handler> logger)
        {
            _photoRepository = photoRepository;
            _parameterRepository = parameterRepository;
            _logger = logger;
        }

        public async Task<PhotoDownloadDto> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 DownloadSinglePhotoQuery - STARTED. PhotoId: {PhotoId}", request.PhotoId);

            _imagePath = await _parameterRepository.GetValueAsync("ImagePath") ?? _imagePath;

            // Validate photo exists in database
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId);
            if (photo == null)
            {
                _logger.LogWarning("⚠️ Photo not found in database: {PhotoId}", request.PhotoId);
                throw new NotFoundException(ValidationErrorMessages.Photo.NotFound(request.PhotoId));
            }

            // Validate physical file exists
            var fullPath = Path.Combine(_imagePath, photo.FileName);
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("⚠️ Photo file not found on disk: {FileName}", photo.FileName);
                throw new NotFoundException(ValidationErrorMessages.Photo.FileNotFound(photo.FileName));
            }

            // Check file accessibility
            try
            {
                using var fileStream = File.OpenRead(fullPath);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("⚠️ File access denied: {FileName}", photo.FileName);
                throw new UnauthorizedAccessException(ValidationErrorMessages.Photo.FileAccessDenied(photo.FileName));
            }

            var fileBytes = await File.ReadAllBytesAsync(fullPath, ct);
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
