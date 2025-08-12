namespace controlmat.Application.Common.Dto;

public class UserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Role { get; set; }
}
