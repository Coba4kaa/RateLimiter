using UserService.Repository;
using UserService.Repository.interfaces;
using UserService.Service.DomainModel;
using UserService.Service.DomainService.interfaces;

namespace UserService.Service.DomainService;

public class UserService(IUserRepository userRepository) : IUserService
{
    public Task<Result<bool>> CreateUser(User user, CancellationToken cancellationToken)
    {
        return userRepository.CreateAsync(user, cancellationToken);
    }

    public Task<Result<User>> GetUserById(int id, CancellationToken cancellationToken)
    {
        return userRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Result<List<User>?>> GetUsersByName(string name, string surname, CancellationToken cancellationToken)
    {
        return userRepository.GetByNameAsync(name, surname, cancellationToken);
    }

    public Task<Result<bool>> UpdateUser(User user, CancellationToken cancellationToken)
    {
        return userRepository.UpdateAsync(user, cancellationToken);
    }

    public Task<Result<bool>> DeleteUser(int id, CancellationToken cancellationToken)
    {
        return userRepository.DeleteAsync(id, cancellationToken);
    }
}
