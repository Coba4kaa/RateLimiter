namespace UserService.Service;

public interface IUserRepository
{
    public Task<bool> CreateAsync(User user);
    public Task<User> GetByIdAsync(int id);
    public Task<IEnumerable<User>> GetByNameAsync(string name, string surname);
    public Task<bool> UpdateAsync(User user);
    public Task<bool> DeleteAsync(int id);
}