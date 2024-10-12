using Grpc.Core;
using UserService.Grpc;
using FluentValidation;
using UserService.Controller.Factory;
using UserService.Service.DomainService;
using User = UserService.Service.DomainModel.User;

namespace UserService.Controller
{
    public class UsergRpcController(IUserService userService, IValidator<User> userValidator, IUserFactory userFactory) : Grpc.UserService.UserServiceBase
    {
        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var user = userFactory.CreateUser(
                request.Login,
                request.Password,
                request.Name,
                request.Surname,
                request.Age
            );

            var validationResult = userValidator.Validate(user);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
            }

            var cancellationToken = context.CancellationToken;
            var success = await userService.CreateUserAsync(user, cancellationToken);

            return new CreateUserResponse
            {
                Success = success,
                Message = success ? "User created successfully" : "User already exists"
            };
        }

        public override async Task<UserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var cancellationToken = context.CancellationToken;
            var user = await userService.GetUserByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with ID {request.Id} not found."));
            }

            return new UserResponse
            {
                User = new Grpc.User
                {
                    Id = user.Id,
                    Login = user.Login,
                    Password = user.Password,
                    Name = user.Name,
                    Surname = user.Surname,
                    Age = user.Age
                }
            };
        }

        public override async Task<UsersResponse> GetUserByName(GetUserByNameRequest request, ServerCallContext context)
        {
            var cancellationToken = context.CancellationToken;
            var users = await userService.GetUsersByNameAsync(request.Name, request.Surname, cancellationToken);
            if (users == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Users with name {request.Name} and surname {request.Surname} not found."));
            }

            var response = new UsersResponse();
            foreach (var user in users)
            {
                response.Users.Add(new Grpc.User
                {
                    Id = user.Id,
                    Login = user.Login,
                    Password = user.Password,
                    Name = user.Name,
                    Surname = user.Surname,
                    Age = user.Age
                });
            }

            return response;
        }

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            var user = new User
            (
                request.Id,
                "unchanged_login",
                request.Password,
                request.Name,
                request.Surname,
                request.Age
            );

            var validationResult = userValidator.Validate(user);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
            }

            var cancellationToken = context.CancellationToken;
            var success = await userService.UpdateUserAsync(user, cancellationToken);

            return new UpdateUserResponse
            {
                Success = success
            };
        }

        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var cancellationToken = context.CancellationToken;
            var success = await userService.DeleteUserAsync(request.Id, cancellationToken);
            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with ID {request.Id} not found."));
            }

            return new DeleteUserResponse
            {
                Success = success
            };
        }
    }
}
