using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using UserService.Grpc;
using UserService.Service.DomainInterface;

namespace UserService.Repository
{
    public class UserRepository
        (IOptions<DatabaseSettings> dbSettings, ILogger<UserRepository> logger) : IUserRepository
    {
        private readonly string _connectionString = dbSettings.Value.UserServiceDb;
        
        public async Task<Result<bool>> CreateAsync(IUser user, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "CALL create_user(@login, @password, @name, @surname, @age)";
            var parameters = new DynamicParameters();
            parameters.Add("login", user.Login);
            parameters.Add("password", user.Password);
            parameters.Add("name", user.Name);
            parameters.Add("surname", user.Surname);
            parameters.Add("age", user.Age);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
                return Result<bool>.Success(true);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while creating user with Login: {Login}", user.Login);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<IUser>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM get_user_by_id(@id)";
            var parameters = new DynamicParameters();
            parameters.Add("id", id);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var dbModel = await connection.QueryFirstOrDefaultAsync<User>(command);
                return dbModel == null ? Result<IUser>.Failure($"User with ID {id} not found") : Result<IUser>.Success(dbModel);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while getting user by ID: {Id}", id);
                return Result<IUser>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<IUser>?>> GetByNameAsync(string name, string surname,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM get_user_by_name(@name, @surname)";
            var parameters = new DynamicParameters();
            parameters.Add("name", name);
            parameters.Add("surname", surname);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var dbModels = await connection.QueryAsync<User>(command);
                var users = dbModels.ToList();
                return users.Count == 0 ? Result<List<IUser>?>.Failure($"No users found with name {name} and surname {surname}") : Result<List<IUser>?>.Success(users.OfType<IUser>().ToList());
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while getting users by name: {Name}, {Surname}", name, surname);
                return Result<List<IUser>?>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(IUser user, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "CALL update_user(@id, @password, @name, @surname, @age)";
            var parameters = new DynamicParameters();
            parameters.Add("id", user.Id);
            parameters.Add("password", user.Password);
            parameters.Add("name", user.Name);
            parameters.Add("surname", user.Surname);
            parameters.Add("age", user.Age);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
                return Result<bool>.Success(true);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while updating user with ID: {Id}", user.Id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "CALL delete_user(@id)";
            var parameters = new DynamicParameters();
            parameters.Add("id", id);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
                return Result<bool>.Success(true);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while deleting user with ID: {Id}", id);
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}