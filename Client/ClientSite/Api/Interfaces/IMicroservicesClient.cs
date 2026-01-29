using Api.Interfaces.MServices.ContentMService;
using Api.Interfaces.MServices.IdentityMService;

namespace Api.Interfaces
{
    public interface IMicroservicesClient
    {
        public IIdentityMService Identity { get; }
        public IContentMService Content { get; }
    }
}