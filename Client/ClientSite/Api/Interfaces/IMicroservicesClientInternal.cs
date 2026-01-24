using Api.Models;

namespace Api.Interfaces
{
    internal interface IMicroservicesClientInternal
    {
        internal Configuration Configuration { get; }
        internal IHttpClientFactory HttpClientFactory { get; }
    }
}