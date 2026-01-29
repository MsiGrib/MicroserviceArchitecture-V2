using Api.Interfaces.MServices.ContentMService.Endpoints;

namespace Api.Interfaces.MServices.ContentMService
{
    public interface IContentMService
    {
        public IPostEndpoints Post { get; }
        public ICommentEndpoints Comment { get; }
        public IReactionEndpoints Reaction { get; }
    }
}