using Dapper;
using UserService.Service;
using Npgsql;

namespace UserService.Repository;

public class UserRepository(string? connectionString) : IUserRepository
{
    public async Task<bool> CreateAsync(User user)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "CALL create_user(@login, @password, @name, @surname, @age)";
        try
        {
            await connection.ExecuteAsync(query, new { user.Login, user.Password, user.Name, user.Surname, user.Age });
            return true;
        }
        catch (PostgresException)
        {
            return false;
        }
    }


    public async Task<User> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "SELECT * FROM get_user_by_id(@id)";
        try
        {
            return await connection.QueryFirstOrDefaultAsync<User>(query, new { id });
        }
        catch (PostgresException)
        {
            return null;
        }
    }


    public async Task<IEnumerable<User>> GetByNameAsync(string name, string surname)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "SELECT * FROM get_user_by_name(@name, @surname)";
        try
        {
            return await connection.QueryAsync<User>(query, new { name, surname });
        }
        catch (PostgresException)
        {
            return null;
        }
    }


    public async Task<bool> UpdateAsync(User user)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "CALL update_user(@id, @password, @name, @surname, @age)";
        try
        {
            await connection.ExecuteAsync(query, new { user.Id, user.Password, user.Name, user.Surname, user.Age });
            return true;
        }
        catch (PostgresException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "CALL delete_user(@id)";
        try
        {
            await connection.ExecuteAsync(query, new { id });
            return true;
        }
        catch (PostgresException)
        {
            return false;
        }
    }
}