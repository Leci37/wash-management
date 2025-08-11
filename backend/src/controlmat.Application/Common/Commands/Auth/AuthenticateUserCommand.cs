using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Commands.Auth
{
    public static class AuthenticateUserCommand
    {
        public class Request : IRequest<LoginResponseDto>
        {
            public LoginRequestDto Credentials { get; set; } = new();
        }

        public class Handler : IRequestHandler<Request, LoginResponseDto>
        {
            private readonly IUserRepository _userRepo;
            private readonly IAuthenticationService _authService;
            private readonly ILogger<Handler> _logger;

            public Handler(
                IUserRepository userRepo,
                IAuthenticationService authService,
                ILogger<Handler> logger)
            {
                _userRepo = userRepo;
                _authService = authService;
                _logger = logger;
            }

            public async Task<LoginResponseDto> Handle(Request request, CancellationToken ct)
            {
                var function = nameof(Handle);
                var threadId = Thread.CurrentThread.ManagedThreadId;

                _logger.LogInformation("{Function} [Thread:{ThreadId}] - LOGIN ATTEMPT. User: {UserName}",
                    function, threadId, request.Credentials.UserName);

                try
                {
                    var user = await _userRepo.GetByUserNameAsync(request.Credentials.UserName);

                    if (user == null || user.IsActive == false)
                    {
                        _logger.LogWarning("{Function} [Thread:{ThreadId}] - INVALID USER. User: {UserName}",
                            function, threadId, request.Credentials.UserName);
                        throw new UnauthorizedAccessException("Invalid credentials");
                    }


                    if (!_authService.ValidatePassword(request.Credentials.Password, user.PasswordHash ?? string.Empty))
                    {
                        _logger.LogWarning("{Function} [Thread:{ThreadId}] - INVALID PASSWORD. User: {UserName}",
                            function, threadId, request.Credentials.UserName);
                        throw new UnauthorizedAccessException("Invalid credentials");
                    }


                    var token = _authService.GenerateJwtToken(user.UserId, user.UserName, user.Role);
                    await _userRepo.UpdateLastLoginAsync(user.UserId);

                    var response = new LoginResponseDto
                    {
                        Token = token,
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Role = user.Role,
                        ExpiresAt = DateTime.UtcNow.AddHours(8),
                        IsAuthenticated = true
                    };

                    _logger.LogInformation("{Function} [Thread:{ThreadId}] - LOGIN SUCCESS. User: {UserName}, Role: {Role}",
                        function, threadId, user.UserName, user.Role);

                    return response;
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("{Function} [Thread:{ThreadId}] - LOGIN FAILED. User: {UserName}",
                        function, threadId, request.Credentials.UserName);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Function} [Thread:{ThreadId}] - LOGIN ERROR. User: {UserName}",
                        function, threadId, request.Credentials.UserName);
                    throw;
                }
            }
        }
    }
}
