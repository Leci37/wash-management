using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Queries.Washing
{
    public static class GetActiveWashesQuery
    {
        public class Request : IRequest<List<WashingResponseDto>> { }

        public class Handler : IRequestHandler<Request, List<WashingResponseDto>>
        {
            private readonly IWashingRepository _washingRepo;
            private readonly IProtRepository _protRepo;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(
                IWashingRepository washingRepo,
                IProtRepository protRepo,
                IMapper mapper,
                ILogger<Handler> logger)
            {
                _washingRepo = washingRepo;
                _protRepo = protRepo;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<List<WashingResponseDto>> Handle(Request request, CancellationToken ct)
            {
                var function = nameof(Handle);
                var threadId = Thread.CurrentThread.ManagedThreadId;

                _logger.LogInformation("\ud83d\udd00 {Function} [Thread:{ThreadId}] - STARTED", function, threadId);

                try
                {
                    var activeWashings = await _washingRepo.GetActiveAsync();
                    var response = new List<WashingResponseDto>();

                    foreach (var washing in activeWashings)
                    {
                        var prots = await _protRepo.GetByWashingIdAsync(washing.WashingId);

                        var washingDto = new WashingResponseDto
                        {
                            WashingId = washing.WashingId,
                            MachineId = washing.MachineId,
                            MachineName = washing.Machine?.Name ?? string.Empty,
                            StartUserId = washing.StartUserId,
                            StartUserName = washing.StartUser?.UserName ?? string.Empty,
                            EndUserId = washing.EndUserId,
                            EndUserName = washing.EndUser?.UserName,
                            StartDate = washing.StartDate,
                            EndDate = washing.EndDate,
                            Status = washing.Status.ToString(),
                            StartObservation = washing.StartObservation,
                            FinishObservation = washing.FinishObservation,
                            Prots = _mapper.Map<List<ProtDto>>(prots),
                            Photos = new List<PhotoDto>()
                        };

                        response.Add(washingDto);
                    }

                    _logger.LogInformation("\u2705 {Function} [Thread:{ThreadId}] - COMPLETED. Count: {Count}",
                        function, threadId, response.Count);

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "\u274c {Function} [Thread:{ThreadId}] - ERROR", function, threadId);
                    throw;
                }
            }
        }
    }
}
