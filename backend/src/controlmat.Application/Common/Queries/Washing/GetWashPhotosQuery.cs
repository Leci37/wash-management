using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Queries.Washing;

public static class GetWashPhotosQuery
{
    public record Request(long WashId) : IRequest<List<PhotoDto>>;

    public class Handler : IRequestHandler<Request, List<PhotoDto>>
    {
        private readonly IPhotoRepository _repository;
        private readonly IWashingRepository _washingRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(IPhotoRepository repository, IWashingRepository washingRepository, ILogger<Handler> logger)
        {
            _repository = repository;
            _washingRepository = washingRepository;
            _logger = logger;
        }

        public async Task<List<PhotoDto>> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 GetWashPhotosQuery - STARTED. WashId: {WashId}", request.WashId);

            if (!IsValidWashingId(request.WashId))
                throw new ValidationException(ValidationErrorMessages.Washing.InvalidIdFormat(request.WashId));

            if (await _washingRepository.GetByIdAsync(request.WashId) is null)
                throw new ValidationException(ValidationErrorMessages.Washing.NotFound(request.WashId));

            var photos = await _repository.GetByWashingIdAsync(request.WashId);
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
            return DateTime.TryParseExact(idStr.Substring(0, 6), "yyMMdd", null,
                System.Globalization.DateTimeStyles.None, out _);
        }
    }
}
