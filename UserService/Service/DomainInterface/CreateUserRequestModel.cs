namespace UserService.Service.DomainInterface;

public record CreateUserRequestModel : IUser
{
    public int Id => 0;
    public string Login { get; init; }
    public string Password { get; init; }
    public string Name { get; init; }
    public string Surname { get; init; }
    public int Age { get; init; }
}