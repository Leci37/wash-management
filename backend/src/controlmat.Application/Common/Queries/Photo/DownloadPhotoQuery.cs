using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using Controlmat.Domain.Repositories;
using System.IO;

namespace Controlmat.Application.Common.Queries.Photo;

public static class DownloadPhotoQuery
{
    public record Request(int PhotoId) : IRequest<PhotoDownloadDto?>;

    public class Handler : IRequestHandler<Request, PhotoDownloadDto?>
    {
        private readonly IPhotoRepository _repository;
        private readonly IParameterRepository _parameterRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(IPhotoRepository repository, IParameterRepository parameterRepository, ILogger<Handler> logger)
        {
            _repository = repository;
            _parameterRepository = parameterRepository;
            _logger = logger;
        }

        public async Task<PhotoDownloadDto?> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 DownloadPhotoQuery - STARTED. PhotoId: {PhotoId}", request.PhotoId);

            var photo = await _repository.GetByIdAsync(request.PhotoId);
            if (photo == null)
            {
                _logger.LogWarning("⚠️ Photo not found: {PhotoId}", request.PhotoId);
                return null;
            }

            var imagePath = await _parameterRepository.GetImagePathAsync();
            var year = photo.CreatedAt.Year.ToString();
            var fullPath = Path.Combine(imagePath, year, photo.FileName);

            if (!File.Exists(fullPath))
            {
                _logger.LogError("❌ Photo file not found on disk: {FilePath}", fullPath);
                return null;
            }

            var fileBytes = await File.ReadAllBytesAsync(fullPath, ct);
            var contentType = Path.GetExtension(photo.FileName).ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            _logger.LogInformation("✅ DownloadPhotoQuery - COMPLETED. File size: {Size} bytes", fileBytes.Length);

            return new PhotoDownloadDto
            {
                FileBytes = fileBytes,
                ContentType = contentType,
                FileName = photo.FileName
            };
        }
    }
}
