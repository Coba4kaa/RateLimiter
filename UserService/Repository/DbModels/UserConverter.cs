using UserService.Controller.Factory;
using UserService.Service.DomainModel;

namespace UserService.Repository.DbModels;

public class UserConverter(IUserFactory userFactory) : IUserConverter
{
    public UserDbModel ConvertToDbModel(User user)
    {
        return new UserDbModel
        {
            Id = user.Id,
            Login = user.Login,
            Password = user.Password,
            Name = user.Name,
            Surname = user.Surname,
            Age = user.Age
        };
    }

    public User ConvertToDomainModel(UserDbModel dbModel)
    {
        return userFactory.CreateUser(
            dbModel.Id,
            dbModel.Login, 
            dbModel.Password, 
            dbModel.Name, 
            dbModel.Surname, 
            dbModel.Age);
    }
}