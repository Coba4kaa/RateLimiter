using UserService.Service.DomainModel;

namespace UserService.Controller.Factory;

public interface IUserFactory
{
    User CreateUser(string login, string password, string name, string surname, int age);
}