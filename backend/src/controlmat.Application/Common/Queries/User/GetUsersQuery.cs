using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using controlmat.Application.Common.Dto;
using controlmat.Domain.Interfaces;

namespace controlmat.Application.Common.Queries.User;

public static class GetUsersQuery
{
    public class Request : IRequest<List<UserDto>> { }

    public class Handler : IRequestHandler<Request, List<UserDto>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepo,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<UserDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var function = nameof(Handle);
            var threadId = Thread.CurrentThread.ManagedThreadId;

            _logger.LogInformation("🌀 {Function} [Thread:{ThreadId}] - STARTED", function, threadId);

            try
            {
                var users = await _userRepo.GetAllAsync();
                var result = _mapper.Map<List<UserDto>>(users);

                _logger.LogInformation("✅ {Function} [Thread:{ThreadId}] - COMPLETED. UserCount: {Count}",
                    function, threadId, result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ {Function} [Thread:{ThreadId}] - ERROR", function, threadId);
                throw;
            }
        }
    }
}
