using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using System.Linq;
using System;

namespace Controlmat.Application.Common.Commands.Washing;

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
            var dto = request.Dto;
            var washing = new Washing
            {
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

            return _mapper.Map<WashingResponseDto>(washing);
        }
    }
}
