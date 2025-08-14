using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using System;

namespace Controlmat.Application.Common.Commands.WashCycle;

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
