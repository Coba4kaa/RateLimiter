using UserService.Repository;
using UserService.Service.DomainModel;

namespace UserService.Service.DomainService;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        return await userRepository.CreateAsync(user, cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<User>?> GetUsersByNameAsync(string name, string surname, CancellationToken cancellationToken)
    {
        return await userRepository.GetByNameAsync(name, surname, cancellationToken);
    }

    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        return await userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        return await userRepository.DeleteAsync(id, cancellationToken);
    }
}
