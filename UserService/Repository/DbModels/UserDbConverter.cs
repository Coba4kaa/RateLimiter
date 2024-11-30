using UserService.Service.DomainInterface;

namespace UserService.Repository.DbModels;

public class UserDbConverter : IUserDbConverter
{
    public UserDbModel ConvertToDbModel(IUser user)
    {
        return new UserDbModel(
            user.Id,
            user.Login,
            user.Password,
            user.Name,
            user.Surname,
            user.Age
        );
    }

    public IUser ConvertToUserModel(UserDbModel dbModel)
    {
        return new UserModel
        {
            Id = dbModel.Id,
            Login = dbModel.Login,
            Password = dbModel.Password,
            Name = dbModel.Name,
            Surname = dbModel.Surname,
            Age = dbModel.Age
        };
    }
}