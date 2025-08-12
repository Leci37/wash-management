using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using System;

namespace Controlmat.Application.Common.Commands.Washing;

public static class AddProtCommand
{
    public class Request : IRequest<Unit>
    {
        public long WashingId { get; set; }
        public AddProtDto Dto { get; set; } = default!;
    }

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IWashingRepository _washingRepo;
        private readonly IProtRepository _protRepo;
        private readonly ILogger<Handler> _logger;

        public Handler(IWashingRepository washingRepo, IProtRepository protRepo, ILogger<Handler> logger)
        {
            _washingRepo = washingRepo;
            _protRepo = protRepo;
            _logger = logger;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var washing = await _washingRepo.GetByIdAsync(request.WashingId)
                ?? throw new InvalidOperationException($"Washing with ID {request.WashingId} not found");

            if (washing.Status == 'F')
                throw new InvalidOperationException($"Washing {request.WashingId} is already finished");

            var prot = new Prot
            {
                WashingId = request.WashingId,
                ProtId = request.Dto.ProtId,
                BatchNumber = request.Dto.BatchNumber,
                BagNumber = request.Dto.BagNumber
            };

            await _protRepo.AddAsync(prot);
            return Unit.Value;
        }
    }
}
