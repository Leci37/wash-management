namespace Controlmat.Domain.Interfaces
{
    public interface IAuthenticationService
    {
        string GenerateJwtToken(int userId, string userName, string role);
        bool ValidatePassword(string password, string hash);
        string HashPassword(string password);
    }
}
