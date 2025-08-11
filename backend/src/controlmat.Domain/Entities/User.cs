namespace Controlmat.Domain.Entities;

public class User
{
    public int UserId { get; private set; }
    public string Username { get; private set; } = null!;
    public string Role { get; private set; } = null!;
}
