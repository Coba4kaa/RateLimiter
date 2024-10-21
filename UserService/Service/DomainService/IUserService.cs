using UserService.Repository;
using UserService.Service.DomainModel;

namespace UserService.Service.DomainService;

public interface IUserService
{
    Task<Result<bool>> CreateUser(User user, CancellationToken cancellationToken);
    Task<Result<User>> GetUserById(int id, CancellationToken cancellationToken);
    Task<Result<List<User>?>> GetUsersByName(string name, string surname, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateUser(User user, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteUser(int id, CancellationToken cancellationToken);
}
