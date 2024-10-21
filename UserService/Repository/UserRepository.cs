using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using UserService.Service.DomainModel;
using UserService.Repository.DbModels;

namespace UserService.Repository
{
    public class UserRepository
        (IOptions<DatabaseSettings> dbSettings, IUserDomainDbConverter userDomainDbConverter, ILogger<UserRepository> logger) : IUserRepository
    {
        private readonly string _connectionString = dbSettings.Value.UserServiceDb;
        
        public async Task<Result<bool>> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var dbModel = userDomainDbConverter.ConvertToDbModel(user);
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "CALL create_user(@login, @password, @name, @surname, @age)";
            var parameters = new DynamicParameters();
            parameters.Add("login", dbModel.Login);
            parameters.Add("password", dbModel.Password);
            parameters.Add("name", dbModel.Name);
            parameters.Add("surname", dbModel.Surname);
            parameters.Add("age", dbModel.Age);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
                return Result<bool>.Success(true);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while creating user with Login: {Login}", dbModel.Login);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<User>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM get_user_by_id(@id)";
            var parameters = new DynamicParameters();
            parameters.Add("id", id);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var dbModel = await connection.QueryFirstOrDefaultAsync<UserDbModel>(command);
                return dbModel == null ? Result<User>.Failure($"User with ID {id} not found") : Result<User>.Success(userDomainDbConverter.ConvertToDomainModel(dbModel));
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while getting user by ID: {Id}", id);
                return Result<User>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<User>?>> GetByNameAsync(string name, string surname,
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
                var dbModels = await connection.QueryAsync<UserDbModel>(command);
                var users = dbModels.Select(userDomainDbConverter.ConvertToDomainModel).ToList();
                return users.Count == 0 ? Result<List<User>?>.Failure($"No users found with name {name} and surname {surname}") : Result<List<User>?>.Success(users);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while getting users by name: {Name}, {Surname}", name, surname);
                return Result<List<User>?>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var dbModel = userDomainDbConverter.ConvertToDbModel(user);
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "CALL update_user(@id, @password, @name, @surname, @age)";
            var parameters = new DynamicParameters();
            parameters.Add("id", dbModel.Id);
            parameters.Add("password", dbModel.Password);
            parameters.Add("name", dbModel.Name);
            parameters.Add("surname", dbModel.Surname);
            parameters.Add("age", dbModel.Age);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
                return Result<bool>.Success(true);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while updating user with ID: {Id}", dbModel.Id);
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