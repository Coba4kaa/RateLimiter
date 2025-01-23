using UserService.Service.DomainInterface;

namespace UserService.Grpc
{
    public partial class UpdateUserRequest : IUser
    {
        public string Login => "unchaged";
    }
}
    