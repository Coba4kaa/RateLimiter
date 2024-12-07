using Microsoft.Extensions.Caching.Memory;
using UserService.Grpc;
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
        var cacheNameSurnameKey = $"{name}_{surname}";
        if (memoryCache.TryGetValue(cacheNameSurnameKey, out List<IUser>? cachedUsers) && cachedUsers.Any())
        {
            Console.WriteLine($"List of users with alias {name} {surname} has been cached earlier.");
            return Task.FromResult(Result<List<IUser>?>.Success(cachedUsers));
        }
        
        var userResult= userRepository.GetByNameAsync(name, surname, cancellationToken);
        if (userResult.Result.IsSuccess && userResult.Result.Value != null)
        {
            userResult.Result.Value.ForEach(user =>
            {
                if (memoryCache.TryGetValue(user.Id, out _))
                {
                    return;
                }
                
                memoryCache.Set(user.Id, user, TimeSpan.FromMinutes(10));
                Console.WriteLine($"{user.Id} now in memory cache.");
            });
            memoryCache.Set(cacheNameSurnameKey, userResult.Result.Value, TimeSpan.FromMinutes(10));
            Console.WriteLine($"List of users with alias {name} {surname} now in memory cache.");
        }
        
        return userResult;
    }

    public Task<Result<bool>> UpdateUser(IUser user, CancellationToken cancellationToken)
    {
        if (!memoryCache.TryGetValue(user.Id, out IUser cachedUser))
        {
            return userRepository.UpdateAsync(user, cancellationToken);
        }
        
        var updatedUser = new User
        {
            Id = user.Id, 
            Login = cachedUser.Login,
            Password = user.Password, 
            Name = user.Name, 
            Surname = user.Surname, 
            Age = user.Age
            
        };
        memoryCache.Set(updatedUser.Id, updatedUser, TimeSpan.FromMinutes(10));
        var cachedNameSurnameKey = $"{updatedUser.Name}_{updatedUser.Surname}";
        if (!memoryCache.TryGetValue(cachedNameSurnameKey, out List<IUser>? cachedUsers))
        {
            return userRepository.UpdateAsync(user, cancellationToken);
            
        }
        
        var userIndex = cachedUsers.FindIndex(u => u.Id == user.Id);
        if (userIndex >= 0)
        {
            cachedUsers[userIndex] = updatedUser;
            memoryCache.Set(cachedNameSurnameKey, cachedUsers, TimeSpan.FromMinutes(10));
        }
        
        return userRepository.UpdateAsync(user, cancellationToken);
    }

    public Task<Result<bool>> DeleteUser(int id, CancellationToken cancellationToken)
    {
        if (!memoryCache.TryGetValue(id, out IUser cachedUser))
        {
            return userRepository.DeleteAsync(id, cancellationToken);
        }
        
        memoryCache.Remove(id);
        var cachedNameSurnameKey = $"{cachedUser.Name}_{cachedUser.Surname}";
        if (!memoryCache.TryGetValue(cachedNameSurnameKey, out List<IUser>? cachedUsers))
        {
            return userRepository.DeleteAsync(id, cancellationToken);
        }
        
        var userIndex = cachedUsers.FindIndex(u => u.Id == id);
        if (userIndex < 0)
        {
            return userRepository.DeleteAsync(id, cancellationToken);
        }
        
        cachedUsers.RemoveAt(userIndex);
        if (cachedUsers.Count == 0)
        {
            memoryCache.Remove(cachedNameSurnameKey);
        }
        else
        {
            memoryCache.Set(cachedNameSurnameKey, cachedUsers, TimeSpan.FromMinutes(10));
        }
        return userRepository.DeleteAsync(id, cancellationToken);
    }
}
