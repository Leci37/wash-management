using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Controlmat.Application.Common.Queries.Washing;

public static class DownloadWashPhotosQuery
{
    public record Request(long WashId) : IRequest<WashPhotosZipDto?>;

    public class Handler : IRequestHandler<Request, WashPhotosZipDto?>
    {
        private readonly IPhotoRepository _repository;
        private readonly ILogger<Handler> _logger;

        public Handler(IPhotoRepository repository, ILogger<Handler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<WashPhotosZipDto?> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 DownloadWashPhotosQuery - STARTED. WashId: {WashId}", request.WashId);

            var photos = (await _repository.GetByWashingIdAsync(request.WashId)).ToList();
            if (!photos.Any())
            {
                _logger.LogWarning("⚠️ No photos found for wash: {WashId}", request.WashId);
                return null;
            }

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var photo in photos)
                {
                    if (File.Exists(photo.FilePath))
                    {
                        var entry = archive.CreateEntry(photo.FileName);
                        using var entryStream = entry.Open();
                        using var fileStream = File.OpenRead(photo.FilePath);
                        await fileStream.CopyToAsync(entryStream, ct);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Photo file not found: {FilePath}", photo.FilePath);
                    }
                }
            }

            var zipBytes = memoryStream.ToArray();
            _logger.LogInformation("✅ DownloadWashPhotosQuery - COMPLETED. ZIP size: {Size} bytes, Photos: {Count}",
                zipBytes.Length, photos.Count);

            return new WashPhotosZipDto { ZipBytes = zipBytes };
        }
    }
}

