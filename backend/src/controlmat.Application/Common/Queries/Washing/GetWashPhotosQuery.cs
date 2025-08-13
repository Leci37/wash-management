using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Controlmat.Application.Common.Queries.Washing;

public static class GetWashPhotosQuery
{
    public record Request(long WashId) : IRequest<List<PhotoDto>>;

    public class Handler : IRequestHandler<Request, List<PhotoDto>>
    {
        private readonly IPhotoRepository _repository;
        private readonly ILogger<Handler> _logger;

        public Handler(IPhotoRepository repository, ILogger<Handler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<PhotoDto>> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 GetWashPhotosQuery - STARTED. WashId: {WashId}", request.WashId);

            var photos = await _repository.GetByWashIdAsync(request.WashId);
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
    }
}
