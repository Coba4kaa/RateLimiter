namespace UserService.Service.DomainModel;

public class User
{
    public User(int id, string login, string password, string name, string surname, int age)
    {
        Id = id;
        Login = login;
        Password = password;
        Name = name;
        Surname = surname;
        Age = age;
    }
    public User(string login, string password, string name, string surname, int age)
    {
        Login = login;
        Password = password;
        Name = name;
        Surname = surname;
        Age = age;
    }

    public int Id { get; }
    public string Login { get; }
    public string Password { get; }
    public string Name { get; }
    public string Surname { get; }
    public int Age { get; }
}