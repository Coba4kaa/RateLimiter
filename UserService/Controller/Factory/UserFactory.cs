using UserService.Grpc;
using User = UserService.Service.DomainModel.User;

namespace UserService.Controller.Factory;

public class UserFactory : IUserFactory
{
    public User CreateUser(CreateUserRequest request)
    {
        return new User(request.Login, request.Password, request.Name, request.Surname, request.Age);
    }
}