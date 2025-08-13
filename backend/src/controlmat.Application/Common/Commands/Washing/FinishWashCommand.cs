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
                var dto = request.Dto;
                _logger.LogInformation("Finishing wash {WashingId} by user {EndUserId}", request.WashingId, dto.EndUserId);

                var washing = await _washingRepo.GetByIdAsync(request.WashingId)
                    ?? throw new InvalidOperationException($"Washing with ID {request.WashingId} not found");

                if (washing.Status == 'F')
                    throw new InvalidOperationException($"Washing {request.WashingId} already finished");

                if (washing.StartUserId == dto.EndUserId)
                {
                    _logger.LogWarning("Same user trying to start and finish wash: {UserId}", dto.EndUserId);
                    throw new InvalidOperationException("The user who finishes the wash must be different from the user who started it");
                }

                var washDuration = DateTime.UtcNow - washing.StartDate;
                if (washDuration.TotalMinutes < 5)
                {
                    _logger.LogWarning("Wash too short: {Duration} minutes", washDuration.TotalMinutes);
                    throw new InvalidOperationException("Wash cycle must run for at least 5 minutes");
                }

                washing.EndUserId = dto.EndUserId;
                washing.FinishObservation = dto.FinishObservation;
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
