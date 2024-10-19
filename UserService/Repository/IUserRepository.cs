using UserService.Service.DomainModel;

namespace UserService.Repository;

public interface IUserRepository
{
    Task<Result<bool>> CreateAsync(User user, CancellationToken cancellationToken);
    Task<Result<User>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<List<User>?>> GetByNameAsync(string name, string surname, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateAsync(User user, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
}
