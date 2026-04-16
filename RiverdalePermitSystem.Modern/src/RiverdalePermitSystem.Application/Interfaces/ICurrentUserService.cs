namespace RiverdalePermitSystem.Application.Interfaces;

public interface ICurrentUserService
{
    Task<string> GetCurrentUserIdAsync();
    Task<string> GetCurrentUserNameAsync();
    Task<IReadOnlyList<string>> GetCurrentUserRolesAsync();
}
