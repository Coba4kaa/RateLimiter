using UserService.Grpc;
using User = UserService.Service.DomainModel.User;

namespace UserService.Controller.Factory;

public class UserDomainControllerConverter : IUserDomainControllerConverter
{
    public User ConvertToDomainModel(CreateUserRequest request)
    {
        return new User(request.Login, request.Password, request.Name, request.Surname, request.Age);
    }

    public Grpc.User ConvertToControllerModel(User user)
    {
        return new Grpc.User
        {
            Id = user.Id,
            Login = user.Login,
            Password = user.Password,
            Name = user.Name,
            Surname = user.Surname,
            Age = user.Age
        };
    }
}