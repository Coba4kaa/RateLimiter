using UserService.Service.DomainModel;

namespace UserService.Service.DomainService;

public interface IUserService
{
    Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<User>?> GetUsersByNameAsync(string name, string surname, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken);
}
