using Grpc.Core;
using UserService.Grpc;
using FluentValidation;
using UserService.Controller.Factory;
using UserService.Service.DomainService;
using User = UserService.Service.DomainModel.User;

namespace UserService.Controller
{
    public class UsergRpcController(IUserService userService, IValidator<User> userValidator, IUserDomainControllerConverter userDomainControllerConverter) : Grpc.UserService.UserServiceBase
    {
        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var user = userDomainControllerConverter.ConvertToDomainModel(request);

            var validationResult = userValidator.Validate(user);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
            }

            var cancellationToken = context.CancellationToken;
            var result = await userService.CreateUser(user, cancellationToken);

            return new CreateUserResponse
            {
                Success = result.IsSuccess,
                Message = result.IsSuccess ? "User created successfully" : result.Error
            };
        }

        public override async Task<UserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var cancellationToken = context.CancellationToken;
            var result = await userService.GetUserById(request.Id, cancellationToken);
            if (!result.IsSuccess)
            {
                return new UserResponse
                {
                    Message = result.Error
                };
            }

            var user = result.Value;

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
                },
                Message = "Found successfully"
            };
        }

        public override async Task<UsersResponse> GetUserByName(GetUserByNameRequest request, ServerCallContext context)
        {
            var cancellationToken = context.CancellationToken;
            var result = await userService.GetUsersByName(request.Name, request.Surname, cancellationToken);
            if (!result.IsSuccess)
            {
                return new UsersResponse
                {
                    Message = result.Error
                };
            }

            var response = new UsersResponse();
            foreach (var user in result.Value)
            {
                response.Users.Add(userDomainControllerConverter.ConvertToControllerModel(user));
            }
            response.Message = "Found successfully";

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
            var result = await userService.UpdateUser(user, cancellationToken);

            return new UpdateUserResponse
            {
                Success = result.IsSuccess,
                Message = result.IsSuccess ? "User updated successfully" : result.Error
            };
        }

        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var cancellationToken = context.CancellationToken;
            var result = await userService.DeleteUser(request.Id, cancellationToken);

            return new DeleteUserResponse
            {
                Success = result.IsSuccess,
                Message = result.IsSuccess ? "User deleted successfully" : result.Error
            };
        }
    }
}
