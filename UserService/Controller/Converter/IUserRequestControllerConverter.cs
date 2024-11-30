using UserService.Grpc;
using UserService.Service.DomainInterface;

namespace UserService.Controller.Converter;

public interface IUserRequestControllerConverter
{
    IUser ConvertToCreateRequestModel(CreateUserRequest request);
    IUser ConvertToUpdateRequestModel(UpdateUserRequest request);
    User ConvertToMessage(IUser user);
}