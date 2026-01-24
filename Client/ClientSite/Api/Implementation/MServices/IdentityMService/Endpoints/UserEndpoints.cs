using Api.Interfaces;
using Api.Interfaces.MServices.IdentityMService.Endpoints;
using Api.Models.MServices.IdentityMService.Endpoints.UserEndpoints.Responses;
using RestSharp;

namespace Api.Implementation.MServices.IdentityMService.Endpoints
{
    internal class UserEndpoints : BaseEndPoint, IUserEndpoints
    {
        public UserEndpoints(IMicroservicesClient client, string basePath)
            : base(client, "Identity", basePath) { }

        public Task<UserDTO> GetProfile(string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl("/me"), Method.Get);
            return ExecuteAsync<UserDTO>(restRequest, token, cancellationToken);
        }

        //public Task<UserProfileResponse> UpdateProfile(
        //    UpdateProfileRequest request,
        //    string token,
        //    CancellationToken cancellationToken = default)
        //{
        //    var restRequest = new RestRequest(BuildUrl("/profile"), Method.Put);
        //    restRequest.AddJsonBody(request);
        //    return ExecuteAsync<UserProfileResponse>(restRequest, token, cancellationToken);
        //}

        //public Task<List<UserResponse>> GetUsers(
        //    GetUsersRequest request,
        //    string token,
        //    CancellationToken cancellationToken = default)
        //{
        //    var restRequest = new RestRequest(BuildUrl(""), Method.Get);

        //    if (request.Page.HasValue)
        //        restRequest.AddQueryParameter("page", request.Page.Value.ToString());
        //    if (request.PageSize.HasValue)
        //        restRequest.AddQueryParameter("pageSize", request.PageSize.Value.ToString());
        //    if (!string.IsNullOrEmpty(request.Search))
        //        restRequest.AddQueryParameter("search", request.Search);

        //    return ExecuteAsync<List<UserResponse>>(restRequest, token, cancellationToken);
        //}
    }
}