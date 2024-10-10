using Grpc.Core;
using UserService.Grpc;
using FluentValidation;
using UserService.Service;

namespace UserService.Controller;

public class UsergRpcController(IUserRepository userRepository, IValidator<Service.User> userValidator) : Grpc.UserService.UserServiceBase
{
    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var user = new Service.User
        (
            request.Login,
            request.Password,
            request.Name,
            request.Surname,
            request.Age
        );

        var validationResult = await userValidator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
        }

        bool success = await userRepository.CreateAsync(user);

        return new CreateUserResponse
        {
            Success = success,
            Message = success ? "User created successfully" : "User already exists"
        };
    }

    public override async Task<UserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
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
        var users = await userRepository.GetByNameAsync(request.Name, request.Surname);
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
        var user = new Service.User
        (
            request.Id,
            "unchanged_login",
            request.Password,
            request.Name,
            request.Surname,
            request.Age
        );

        var validationResult = await userValidator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
        }

        bool success = await userRepository.UpdateAsync(user);

        return new UpdateUserResponse
        {
            Success = success
        };
    }

    public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        bool success = await userRepository.DeleteAsync(request.Id);
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
