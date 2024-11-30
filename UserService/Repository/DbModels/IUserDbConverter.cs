using UserService.Service.DomainInterface;

namespace UserService.Repository.DbModels;

public interface IUserDbConverter
{
    UserDbModel ConvertToDbModel(IUser user);
    IUser ConvertToUserModel(UserDbModel dbModel);
}
