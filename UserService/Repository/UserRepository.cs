using Dapper;
using Npgsql;
using UserService.Service.DomainModel;
using UserService.Repository.DbModels;

namespace UserService.Repository
{
    public class UserRepository
        (string connectionString, IUserConverter userConverter, ILogger<UserRepository> logger) : IUserRepository
    {
        public async Task<Result<bool>> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var dbModel = userConverter.ConvertToDbModel(user);
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
            var query = "SELECT * FROM get_user_by_id(@id)";
            var parameters = new DynamicParameters();
            parameters.Add("id", id);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var dbModel = await connection.QueryFirstOrDefaultAsync<UserDbModel>(command);
                return Result<User>.Success(userConverter.ConvertToDomainModel(dbModel));
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
            await using var connection = new NpgsqlConnection(connectionString);
            var query = "SELECT * FROM get_user_by_name(@name, @surname)";
            var parameters = new DynamicParameters();
            parameters.Add("name", name);
            parameters.Add("surname", surname);

            try
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var dbModels = await connection.QueryAsync<UserDbModel>(command);
                var users = dbModels.Select(userConverter.ConvertToDomainModel).ToList();
                return Result<List<User>?>.Success(users);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Error while getting users by name: {Name}, {Surname}", name, surname);
                return Result<List<User>?>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var dbModel = userConverter.ConvertToDbModel(user);
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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