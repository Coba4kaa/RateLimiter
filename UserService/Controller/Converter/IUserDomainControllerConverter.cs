using UserService.Grpc;
using User = UserService.Service.DomainModel.User;

namespace UserService.Controller.Factory;

public interface IUserDomainControllerConverter
{
    User ConvertToDomainModel(CreateUserRequest request);
    Grpc.User ConvertToControllerModel(User user);
}