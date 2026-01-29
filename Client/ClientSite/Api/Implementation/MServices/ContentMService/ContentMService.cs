using Api.Implementation.MServices.ContentMService.Endpoints;
using Api.Interfaces;
using Api.Interfaces.MServices.ContentMService;
using Api.Interfaces.MServices.ContentMService.Endpoints;
using Api.Models;
using Microsoft.Extensions.Options;

namespace Api.Implementation.MServices.ContentMService
{
    internal class ContentMService : IContentMService
    {
        private readonly IMicroservicesClient _client;
        private readonly Configuration _configuration;

        private IPostEndpoints? _postEndpoints;
        private ICommentEndpoints? _commentEndpoints;
        private IReactionEndpoints? _reactionEndpoints;

        public ContentMService(IMicroservicesClient client, IOptions<Configuration> configuration)
        {
            _client = client;
            _configuration = configuration.Value;
        }

        public IPostEndpoints Post
        {
            get
            {
                if (_postEndpoints == null)
                {
                    if (!_configuration.Services.TryGetValue("Content", out var serviceConfig))
                        throw new ArgumentException("Content service not found in configuration");

                    if (!serviceConfig.Controllers.TryGetValue("Post", out var controllerConfig))
                        throw new ArgumentException("Post controller not found in Content service configuration");

                    _postEndpoints = new PostEndpoints(_client, controllerConfig.BasePath);
                }

                return _postEndpoints;
            }
        }

        public ICommentEndpoints Comment
        {
            get
            {
                if (_commentEndpoints == null)
                {
                    if (!_configuration.Services.TryGetValue("Content", out var serviceConfig))
                        throw new ArgumentException("Content service not found in configuration");

                    if (!serviceConfig.Controllers.TryGetValue("Comment", out var controllerConfig))
                        throw new ArgumentException("Comment controller not found in Content service configuration");

                    _commentEndpoints = new CommentEndpoints(_client, controllerConfig.BasePath);
                }

                return _commentEndpoints;
            }
        }

        public IReactionEndpoints Reaction
        {
            get
            {
                if (_reactionEndpoints == null)
                {
                    if (!_configuration.Services.TryGetValue("Content", out var serviceConfig))
                        throw new ArgumentException("Content service not found in configuration");

                    if (!serviceConfig.Controllers.TryGetValue("Reaction", out var controllerConfig))
                        throw new ArgumentException("Reaction controller not found in Content service configuration");

                    _reactionEndpoints = new ReactionEndpoints(_client, controllerConfig.BasePath);
                }

                return _reactionEndpoints;
            }
        }
    }
}