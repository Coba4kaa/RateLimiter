using UserService.Service.DomainModel;

namespace UserService.Repository.DbModels;

public interface IUserConverter
{
    UserDbModel ConvertToDbModel(User user);
    User ConvertToDomainModel(UserDbModel dbModel);
}
