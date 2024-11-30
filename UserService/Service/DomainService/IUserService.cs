using UserService.Repository;
using UserService.Service.DomainInterface;

namespace UserService.Service.DomainService;

public interface IUserService
{
    Task<Result<bool>> CreateUser(IUser user, CancellationToken cancellationToken);
    Task<Result<IUser>> GetUserById(int id, CancellationToken cancellationToken);
    Task<Result<List<IUser>?>> GetUsersByName(string name, string surname, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateUser(IUser user, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteUser(int id, CancellationToken cancellationToken);
}
