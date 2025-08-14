using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using System.Text.RegularExpressions;
using System.Globalization;
using System;

namespace Controlmat.Application.Common.Queries.Washing;

public static class GetWashPhotosQuery
{
    public record Request(long WashId) : IRequest<List<PhotoDto>>;

    public class Handler : IRequestHandler<Request, List<PhotoDto>>
    {
        private readonly IPhotoRepository _photoRepo;
        private readonly IWashingRepository _washingRepo;
        private readonly ILogger<Handler> _logger;

        public Handler(IPhotoRepository photoRepo, IWashingRepository washingRepo, ILogger<Handler> logger)
        {
            _photoRepo = photoRepo;
            _washingRepo = washingRepo;
            _logger = logger;
        }

        public async Task<List<PhotoDto>> Handle(Request request, CancellationToken ct)
        {
            var washingId = request.WashId;
            _logger.LogInformation("🌀 GetWashPhotosQuery - STARTED. WashId: {WashId}", washingId);

            if (!IsValidWashingId(washingId))
                throw new ArgumentException(ValidationErrorMessages.Washing.InvalidIdFormat(washingId));

            var washing = await _washingRepo.GetByIdAsync(washingId);
            if (washing == null)
                throw new InvalidOperationException(ValidationErrorMessages.Washing.NotFound(washingId));

            var photos = await _photoRepo.GetByWashingIdAsync(washingId);
            var result = photos.Select(p => new PhotoDto
            {
                Id = p.Id,
                FileName = p.FileName,
                CreatedAt = p.CreatedAt,
                DownloadUrl = $"/api/photos/{p.Id}/download"
            }).ToList();

            _logger.LogInformation("✅ GetWashPhotosQuery - COMPLETED. Found {Count} photos", result.Count);
            return result;
        }

        private static bool IsValidWashingId(long washingId)
        {
            var idStr = washingId.ToString();
            if (!Regex.IsMatch(idStr, @"^\d{8}$"))
                return false;
            return DateTime.TryParseExact(idStr[..6], "yyMMdd", null, DateTimeStyles.None, out _);
        }
    }
}
