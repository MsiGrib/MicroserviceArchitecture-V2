using Api.Interfaces.MServices.IdentityMService.Endpoints;

namespace Api.Interfaces.MServices.IdentityMService
{
    public interface IIdentityMService
    {
        public IAuthEndpoints Auth { get; }
        public IUserEndpoints User { get; }
    }
}