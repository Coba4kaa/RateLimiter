namespace UserService.Service.DomainModel;

public interface IUserRepository
{
    Task<bool> CreateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<User>?> GetByNameAsync(string name, string surname, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
