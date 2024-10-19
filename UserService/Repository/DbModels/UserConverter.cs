using UserService.Service.DomainModel;

namespace UserService.Repository.DbModels;

public class UserConverter : IUserConverter
{
    public UserDbModel ConvertToDbModel(User user)
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

    public User ConvertToDomainModel(UserDbModel dbModel)
    {
        return new User(
            dbModel.Id,
            dbModel.Login, 
            dbModel.Password, 
            dbModel.Name, 
            dbModel.Surname, 
            dbModel.Age);
    }
}