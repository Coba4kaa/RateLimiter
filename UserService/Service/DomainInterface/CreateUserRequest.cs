using UserService.Service.DomainInterface;

namespace UserService.Grpc
{
    public partial class CreateUserRequest : IUser
    {
        public int Id => 0;
    }
}