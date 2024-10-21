using UserService.Service.DomainModel;

namespace UserService.Repository.DbModels;

public interface IUserDomainDbConverter
{
    UserDbModel ConvertToDbModel(User user);
    User ConvertToDomainModel(UserDbModel? dbModel);
}
