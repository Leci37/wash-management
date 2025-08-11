namespace controlmat.Application.Common.Dto;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? Role { get; set; }
}

