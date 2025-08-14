using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using System.Linq;
using System;
using System.Threading.Tasks;
using WashingEntity = Controlmat.Domain.Entities.Washing;

namespace Controlmat.Application.Common.Commands.WashCycle;

public static class StartWashCommand
{
    public class Request : IRequest<WashingResponseDto>
    {
        public NewWashDto Dto { get; set; } = default!;
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
                _logger.LogInformation("Starting wash: MachineId={MachineId}, StartUserId={StartUserId}", dto.MachineId, dto.StartUserId);
                var washingId = await GenerateWashingIdAsync();

                var washing = new WashingEntity
                {
                    WashingId = washingId,
                    MachineId = dto.MachineId,
                    StartUserId = dto.StartUserId,
                    StartDate = DateTime.UtcNow,
                    Status = 'P',
                    StartObservation = dto.StartObservation,
                    Prots = dto.ProtEntries.Select(p => new Prot
                    {
                        ProtId = p.ProtId,
                        BatchNumber = p.BatchNumber,
                        BagNumber = p.BagNumber
                    }).ToList()
                };

                await _washingRepo.AddAsync(washing);
                _logger.LogInformation("Wash {WashingId} started successfully", washing.WashingId);

                return _mapper.Map<WashingResponseDto>(washing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting wash");
                throw;
            }
        }

        /// <summary>
        /// Generates a unique WashingId in YYMMDDXX format
        /// Where XX is a sequential number for the day (01, 02, 03...)
        /// </summary>
        private async Task<long> GenerateWashingIdAsync()
        {
            var today = DateTime.UtcNow;
            var datePrefix = today.ToString("yyMMdd"); // e.g., "250813"

            // Find the highest existing ID for today
            var existingIds = await _washingRepo.GetWashingIdsByDatePrefixAsync(datePrefix);

            var nextSequence = 1;
            if (existingIds.Any())
            {
                // Extract the XX part and find the next sequence
                var sequences = existingIds
                    .Select(id => id % 100) // Get last 2 digits
                    .Where(seq => seq > 0)  // Ignore invalid sequences
                    .ToList();

                if (sequences.Any())
                {
                    nextSequence = sequences.Max() + 1;
                }
            }

            // Ensure sequence doesn't exceed 99
            if (nextSequence > 99)
            {
                throw new InvalidOperationException($"Maximum washes per day (99) exceeded for {today:yyyy-MM-dd}");
            }

            // Combine date prefix + sequence
            var washingIdString = $"{datePrefix}{nextSequence:D2}";

            if (!long.TryParse(washingIdString, out var washingId))
            {
                throw new InvalidOperationException($"Failed to generate valid WashingId from: {washingIdString}");
            }

            return washingId;
        }
    }
}
