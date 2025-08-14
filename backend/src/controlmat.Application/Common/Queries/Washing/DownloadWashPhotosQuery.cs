using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Dto;
using Controlmat.Application.Common.Exceptions;
using Controlmat.Domain.Interfaces;
using System.IO;
using System.IO.Compression;

namespace Controlmat.Application.Common.Queries.Washing;

public static class DownloadWashPhotosQuery
{
    public record Request(long WashId) : IRequest<WashPhotosZipDto?>;

    public class Handler : IRequestHandler<Request, WashPhotosZipDto?>
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IWashingRepository _washingRepository;
        private readonly IParameterRepository _parameterRepository;
        private readonly ILogger<Handler> _logger;
        private string _imagePath = "C\\SumiSan\\Photos";

        public Handler(IPhotoRepository photoRepository, IWashingRepository washingRepository, IParameterRepository parameterRepository, ILogger<Handler> logger)
        {
            _photoRepository = photoRepository;
            _washingRepository = washingRepository;
            _parameterRepository = parameterRepository;
            _logger = logger;
        }

        public async Task<WashPhotosZipDto?> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 DownloadWashPhotosQuery - STARTED. WashId: {WashId}", request.WashId);

            var washIdStr = request.WashId.ToString();
            if (washIdStr.Length != 8 || !DateTime.TryParseExact(washIdStr.Substring(0, 6), "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                throw new ValidationException(ValidationErrorMessages.Washing.InvalidIdFormat(request.WashId));
            }

            var washing = await _washingRepository.GetByIdAsync(request.WashId);
            if (washing == null)
            {
                throw new ValidationException(ValidationErrorMessages.Washing.NotFound(request.WashId));
            }

            var photoCount = await _photoRepository.CountByWashingIdAsync(request.WashId);
            if (photoCount == 0)
            {
                throw new ValidationException(ValidationErrorMessages.Photo.NoPhotosForWash(request.WashId));
            }

            _imagePath = await _parameterRepository.GetValueAsync("ImagePath") ?? _imagePath;

            var photos = await _photoRepository.GetByWashingIdAsync(request.WashId);

            // Validate all photo files exist before creating ZIP
            foreach (var photo in photos)
            {
                var fullPath = Path.Combine(_imagePath, photo.FileName);
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("⚠️ Photo file missing for ZIP: {FileName}", photo.FileName);
                    throw new NotFoundException(ValidationErrorMessages.Photo.FileNotFound(photo.FileName));
                }
            }

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var photo in photos)
                {
                    var fullPath = Path.Combine(_imagePath, photo.FileName);
                    var entry = archive.CreateEntry(photo.FileName);
                    using var entryStream = entry.Open();
                    using var fileStream = File.OpenRead(fullPath);
                    await fileStream.CopyToAsync(entryStream, ct);
                }
            }

            var zipBytes = memoryStream.ToArray();
            _logger.LogInformation("✅ DownloadWashPhotosQuery - COMPLETED. ZIP size: {Size} bytes, Photos: {Count}",
                zipBytes.Length, photos.Count);

            return new WashPhotosZipDto { ZipBytes = zipBytes };
        }
    }
}

