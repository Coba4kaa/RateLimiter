using Dapper;
using Npgsql;
using UserService.Service.DomainModel;
using UserService.Repository.DbModels;

namespace UserService.Repository
{
    public class UserRepository(string connectionString, IUserConverter userConverter) : IUserRepository
    {
        public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken)
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
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var dbModel = await connection.QueryFirstOrDefaultAsync<UserDbModel>(command);
                return dbModel != null ? userConverter.ConvertToDomainModel(dbModel) : null;
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
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var dbModels = await connection.QueryAsync<UserDbModel>(command);
                return dbModels.Select(userConverter.ConvertToDomainModel);
            }
            catch (PostgresException)
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
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
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
                return true;
            }
            catch (PostgresException)
            {
                return false;
            }
        }
    }
}
