using UserService.Grpc;
using UserService.Service.DomainInterface;
using CreateUserRequest = UserService.Grpc.CreateUserRequest;

namespace UserService.Controller.Converter;

public class UserRequestControllerConverter : IUserRequestControllerConverter
{
    public IUser ConvertToCreateRequestModel(CreateUserRequest request)
    {
        return new CreateUserRequestModel
        {
            Login = request.Login,
            Password = request.Password,
            Name = request.Name,
            Surname = request.Surname,
            Age = request.Age
        };
    }
    
    public IUser ConvertToUpdateRequestModel(UpdateUserRequest request)
    {
        return new UpdateUserRequestModel
        {
            Id = request.Id,
            Password = request.Password,
            Name = request.Name,
            Surname = request.Surname,
            Age = request.Age
        };
    }

    public User ConvertToMessage(IUser user)
    {
        return new User
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