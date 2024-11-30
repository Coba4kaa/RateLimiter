using Microsoft.Extensions.Caching.Memory;
using UserService.Repository;
using UserService.Service.DomainInterface;

namespace UserService.Service.DomainService;

public class UserService(IUserRepository userRepository, IMemoryCache memoryCache) : IUserService
{
    public Task<Result<bool>> CreateUser(IUser user, CancellationToken cancellationToken)
    {
        return userRepository.CreateAsync(user, cancellationToken);
    }

    public Task<Result<IUser>> GetUserById(int id, CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(id, out IUser cachedUser))
        {
            Console.WriteLine($"{id} has been cached earlier. {cachedUser}");
            return Task.FromResult(Result<IUser>.Success(cachedUser));
        }
        
        var userResult = userRepository.GetByIdAsync(id, cancellationToken);
        if (userResult.Result.IsSuccess)
        {
            memoryCache.Set(id, userResult.Result.Value, TimeSpan.FromMinutes(10));
            Console.WriteLine($"{id} now in memory cache.");
        }
        
        return userResult;
    }

    public Task<Result<List<IUser>?>> GetUsersByName(string name, string surname, CancellationToken cancellationToken)
    {
        var userResult = userRepository.GetByNameAsync(name, surname, cancellationToken);
        if (userResult.Result.IsSuccess && userResult.Result.Value != null)
        {
            userResult.Result.Value.ForEach(user =>
            {
                memoryCache.Set(user.Id, user, TimeSpan.FromMinutes(10));
                Console.WriteLine($"{user.Id} now in memory cache.");
            });
        }
        return userResult;
    }

    public Task<Result<bool>> UpdateUser(IUser user, CancellationToken cancellationToken)
    {
        memoryCache.Remove(user.Id);
        return userRepository.UpdateAsync(user, cancellationToken);
    }

    public Task<Result<bool>> DeleteUser(int id, CancellationToken cancellationToken)
    {
        memoryCache.Remove(id);
        return userRepository.DeleteAsync(id, cancellationToken);
    }
}
