namespace UserService.Service.DomainInterface;

public record UserDbModel : IUser
{
    public int Id { get; init; }
    public string Login { get; init; }
    public string Password { get; init; }
    public string Name { get; init; }
    public string Surname { get; init; }
    public int Age { get; init; }
}