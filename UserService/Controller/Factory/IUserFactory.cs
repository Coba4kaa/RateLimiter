using UserService.Grpc;
using User = UserService.Service.DomainModel.User;

namespace UserService.Controller.Factory;

public interface IUserFactory
{
    User CreateUser(CreateUserRequest request);
}