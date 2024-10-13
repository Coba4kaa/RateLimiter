using UserService.Service.DomainModel;

namespace UserService.Controller.Factory;

public class UserFactory : IUserFactory
{
    public User CreateUser(int id, string login, string password, string name, string surname, int age)
    {
        return new User(id, login, password, name, surname, age);
    }
    public User CreateUser(string login, string password, string name, string surname, int age)
    {
        return new User(login, password, name, surname, age);
    }
}