using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Exceptions;
using Controlmat.Domain.Interfaces;
using System;
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
                var message = ValidationErrorMessages.Photo.NotFound(request.PhotoId);
                _logger.LogWarning("⚠️ {Message}", message);
                throw new ValidationException(message);
            }

            var photo = await _repository.GetByIdAsync(request.PhotoId);

            if (!File.Exists(photo.FilePath))
            {
                var message = ValidationErrorMessages.Photo.FileNotFound(photo.FileName);
                _logger.LogError("❌ {Message}", message);
                throw new ValidationException(message);
            }

            byte[] fileBytes;
            try
            {
                fileBytes = await File.ReadAllBytesAsync(photo.FilePath, ct);
            }
            catch (UnauthorizedAccessException)
            {
                var message = ValidationErrorMessages.Photo.FileAccessDenied(photo.FileName);
                _logger.LogError("❌ {Message}", message);
                throw new ValidationException(message);
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
