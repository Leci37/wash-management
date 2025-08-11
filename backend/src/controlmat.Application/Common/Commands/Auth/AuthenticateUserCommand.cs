using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using controlmat.Application.Common.Dto;
using controlmat.Domain.Interfaces;
using FluentValidation;

namespace controlmat.Application.Common.Commands.Auth;

public static class AuthenticateUserCommand
{
    public record Request(LoginRequestDto Dto) : IRequest<LoginResponseDto>;

    public class Handler : IRequestHandler<Request, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<LoginRequestDto> _validator;
        private readonly IConfiguration _configuration;

        public Handler(IUserRepository userRepository, IValidator<LoginRequestDto> validator, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _validator = validator;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.Dto, cancellationToken);

            var user = await _userRepository.GetByUserNameAsync(request.Dto.UserName);
            if (user == null || user.PasswordHash == null || !BCrypt.Verify(request.Dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role ?? "WarehouseUser")
            };

            var jwtSection = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.UserId,
                UserName = user.UserName,
                Role = user.Role
            };
        }
    }
}

