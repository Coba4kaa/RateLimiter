namespace UserService.Service.DomainInterface;

public record UpdateUserRequestModel : IUser
{
    public int Id { get; init; }
    public string Login => "unchaged";
    public string Password { get; init; }
    public string Name { get; init; }
    public string Surname { get; init; }
    public int Age { get; init; }
}