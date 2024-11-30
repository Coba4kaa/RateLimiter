namespace UserService.Service.DomainInterface;

public interface IUser
{
    int Id { get; }
    string Login { get; }
    string Password { get; }
    string Name { get; }
    string Surname { get; }
    int Age { get; }
}