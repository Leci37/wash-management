using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using System;

namespace Controlmat.Application.Common.Commands.Washing;

public static class FinishWashCommand
{
    public class Request : IRequest<WashingResponseDto>
    {
        public long WashingId { get; set; }
        public FinishWashDto Dto { get; set; } = default!;
    }

    public class Handler : IRequestHandler<Request, WashingResponseDto>
    {
        private readonly IWashingRepository _washingRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IWashingRepository washingRepo, IMapper mapper, ILogger<Handler> logger)
        {
            _washingRepo = washingRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WashingResponseDto> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Finishing wash {WashingId} by user {EndUserId}", request.WashingId, request.Dto.EndUserId);

                var washing = await _washingRepo.GetByIdAsync(request.WashingId)
                    ?? throw new InvalidOperationException($"Washing with ID {request.WashingId} not found");

                if (washing.Status == 'F')
                    throw new InvalidOperationException($"Washing {request.WashingId} already finished");

                // Validate StartUser != EndUser
                if (washing.StartUserId == request.Dto.EndUserId)
                {
                    _logger.LogWarning("Same user trying to start and finish wash: {UserId}", request.Dto.EndUserId);
                    throw new InvalidOperationException("The user who finishes the wash must be different from the user who started it");
                }

                // Check wash duration limits
                var washDuration = DateTime.UtcNow - washing.StartDate;
                if (washDuration < TimeSpan.FromSeconds(1))
                {
                    _logger.LogWarning("Wash too short: {Duration} seconds", washDuration.TotalSeconds);
                    throw new InvalidOperationException("Wash cycle must run for at least 1 second");
                }

                washing.EndUserId = request.Dto.EndUserId;
                washing.FinishObservation = request.Dto.FinishObservation;
                washing.EndDate = DateTime.UtcNow;
                washing.Status = 'F';

                await _washingRepo.UpdateAsync(washing);

                _logger.LogInformation("Wash {WashingId} finished", washing.WashingId);

                return _mapper.Map<WashingResponseDto>(washing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finishing wash {WashingId}", request.WashingId);
                throw;
            }
        }
    }
}
