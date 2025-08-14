using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Controlmat.Application.Common.Dto;
using Controlmat.Application.Common.Constants;
using Controlmat.Domain.Interfaces;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Controlmat.Application.Common.Queries.Washing;

public static class DownloadPhotosZipQuery
{
    public record Request(long WashId) : IRequest<WashPhotosZipDto>;

    public class Handler : IRequestHandler<Request, WashPhotosZipDto>
    {
        private readonly IPhotoRepository _repository;
        private readonly IWashingRepository _washingRepo;
        private readonly ILogger<Handler> _logger;

        public Handler(IPhotoRepository repository, IWashingRepository washingRepo, ILogger<Handler> logger)
        {
            _repository = repository;
            _washingRepo = washingRepo;
            _logger = logger;
        }

        public async Task<WashPhotosZipDto> Handle(Request request, CancellationToken ct)
        {
            var washingId = request.WashId;
            _logger.LogInformation("🌀 DownloadPhotosZipQuery - STARTED. WashId: {WashId}", washingId);

            if (!IsValidWashingId(washingId))
                throw new ValidationException(ValidationErrorMessages.Washing.InvalidIdFormat(washingId));

            if (await _washingRepo.GetByIdAsync(washingId) is null)
                throw new ValidationException(ValidationErrorMessages.Washing.NotFound(washingId));

            var photos = await _repository.GetByWashingIdAsync(washingId);
            if (!photos.Any())
                throw new ValidationException(ValidationErrorMessages.Photo.NoPhotosForWash(washingId));

            foreach (var photo in photos)
            {
                if (!File.Exists(photo.FilePath))
                    throw new ValidationException(ValidationErrorMessages.Photo.FileNotFound(photo.FileName));
            }

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var photo in photos)
                {
                    var entry = archive.CreateEntry(photo.FileName);
                    using var entryStream = entry.Open();
                    using var fileStream = File.OpenRead(photo.FilePath);
                    await fileStream.CopyToAsync(entryStream, ct);
                }
            }

            var zipBytes = memoryStream.ToArray();
            _logger.LogInformation("✅ DownloadPhotosZipQuery - COMPLETED. ZIP size: {Size} bytes, Photos: {Count}",
                zipBytes.Length, photos.Count);

            return new WashPhotosZipDto { ZipBytes = zipBytes };
        }

        private static bool IsValidWashingId(long washingId)
        {
            var idStr = washingId.ToString();
            if (!Regex.IsMatch(idStr, @"^\d{8}$"))
                return false;
            return DateTime.TryParseExact(idStr.Substring(0, 6), "yyMMdd", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }
}

