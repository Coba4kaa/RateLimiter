using UserService.Service.DomainInterface;

namespace UserService.Repository;

public interface IUserRepository
{
    Task<Result<bool>> CreateAsync(IUser user, CancellationToken cancellationToken);
    Task<Result<IUser>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<List<IUser>?>> GetByNameAsync(string name, string surname, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateAsync(IUser user, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
}
