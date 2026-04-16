using RiverdalePermitSystem.Application.Interfaces;

namespace RiverdalePermitSystem.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public Task<string> GetCurrentUserIdAsync() => Task.FromResult("demo-user-001");
    public Task<string> GetCurrentUserNameAsync() => Task.FromResult("Demo User");
    public Task<IReadOnlyList<string>> GetCurrentUserRolesAsync() =>
        Task.FromResult<IReadOnlyList<string>>(new List<string> { "Admin", "Inspector" });
}
