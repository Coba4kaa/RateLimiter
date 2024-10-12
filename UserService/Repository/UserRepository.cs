using Dapper;
using Npgsql;
using UserService.Service.DomainModel;

namespace UserService.Repository;

public class UserRepository(string connectionString) : IUserRepository
{
    public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "CALL create_user(@login, @password, @name, @surname, @age)";
        var parameters = new DynamicParameters();
        parameters.Add("login", user.Login);
        parameters.Add("password", user.Password);
        parameters.Add("name", user.Name);
        parameters.Add("surname", user.Surname);
        parameters.Add("age", user.Age);

        try
        {
            await connection.ExecuteAsync(query, parameters);
            return true;
        }
        catch (PostgresException)
        {
            return false;
        }
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "SELECT * FROM get_user_by_id(@id)";
        var parameters = new DynamicParameters();
        parameters.Add("id", id);

        try
        {
            return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
        }
        catch (PostgresException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<User>?> GetByNameAsync(string name, string surname, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "SELECT * FROM get_user_by_name(@name, @surname)";
        var parameters = new DynamicParameters();
        parameters.Add("name", name);
        parameters.Add("surname", surname);

        try
        {
            return await connection.QueryAsync<User>(query, parameters);
        }
        catch (PostgresException)
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "CALL update_user(@id, @password, @name, @surname, @age)";
        var parameters = new DynamicParameters();
        parameters.Add("id", user.Id);
        parameters.Add("password", user.Password);
        parameters.Add("name", user.Name);
        parameters.Add("surname", user.Surname);
        parameters.Add("age", user.Age);

        try
        {
            await connection.ExecuteAsync(query, parameters);
            return true;
        }
        catch (PostgresException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "CALL delete_user(@id)";
        var parameters = new DynamicParameters();
        parameters.Add("id", id);

        try
        {
            await connection.ExecuteAsync(query, parameters);
            return true;
        }
        catch (PostgresException)
        {
            return false;
        }
    }
}

