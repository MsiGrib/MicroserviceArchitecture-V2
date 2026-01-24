using Api.Interfaces.MServices.IdentityMService;
using Api.Models;

namespace Api.Interfaces
{
    public interface IMicroservicesClient
    {
        public IIdentityMService Identity { get; }
    }
}