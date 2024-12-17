using UserService.Service.DomainInterface;

namespace UserService.Repository.Models;

public class UserDbModel : IUser
{
    public int Id { get; init; }
    public string Login { get; init; }
    public string Password { get; init; }
    public string Name { get; init; }
    public string Surname { get; init; }
    public int Age { get; init; }
    
    public override string ToString()
    {
        return $"Id: {Id}, Login: {Login}, Password: {Password}, Name: {Name}, Surname: {Surname}, Age: {Age}";
    }
}