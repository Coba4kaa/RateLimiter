using UserService.Service.DomainModel;

namespace UserService.Controller.Factory;

public class UserFactory : IUserFactory
{
    public User CreateUser(string login, string password, string name, string surname, int age)
    {
        return new User(login, password, name, surname, age);
    }
}