namespace UserService.Repository.DbModels
{
    public record UserDbModel(
        int Id,
        string Login,
        string Password,
        string Name,
        string Surname,
        int Age
    );
}