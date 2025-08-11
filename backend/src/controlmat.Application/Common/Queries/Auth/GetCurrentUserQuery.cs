using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Queries.Auth
{
    public static class GetCurrentUserQuery
    {
        public class Request : IRequest<UserProfileDto>
        {
            public int UserId { get; set; }
        }

        public class Handler : IRequestHandler<Request, UserProfileDto>
        {
            private readonly IUserRepository _userRepo;
            private readonly ILogger<Handler> _logger;

            public Handler(IUserRepository userRepo, ILogger<Handler> logger)
            {
                _userRepo = userRepo;
                _logger = logger;
            }

            public async Task<UserProfileDto> Handle(Request request, CancellationToken ct)
            {
                var user = await _userRepo.GetByIdAsync(request.UserId);
                if (user == null)
                    throw new UnauthorizedAccessException("User not found");

                return new UserProfileDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    LastLogin = user.LastLogin,
                    Permissions = GetPermissions(user.Role)
                };
            }

            private static List<string> GetPermissions(string role)
            {
                return role switch
                {
                    "WarehouseUser" => new List<string> { "wash.start", "wash.finish", "wash.view" },
                    "Supervisor" => new List<string> { "wash.start", "wash.finish", "wash.view", "reports.view" },
                    "Administrator" => new List<string> { "wash.start", "wash.finish", "wash.view", "reports.view", "users.manage" },
                    _ => new List<string>()
                };
            }
        }
    }
}
