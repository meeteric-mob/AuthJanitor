using System;
using System.Threading.Tasks;

namespace AuthJanitor.IdentityServices
{
    public class LocalDebugIdentityServiceDecorator : IIdentityService
    {
        private readonly IIdentityService _service;

        public LocalDebugIdentityServiceDecorator(IIdentityService service)
        {
            _service = service;
            UserRoles = new string[] { AuthJanitorRoles.GlobalAdmin };
        }

        public bool IsUserLoggedIn => true;

        public string UserEmail => "user@simulated.local";

        public string UserName => "Simulated User";

        public string[] UserRoles { get; }

        public bool CurrentUserHasRole(string authJanitorRole)
        {
            return true;
        }

        public async Task<AccessTokenCredential> GetAccessTokenForApplicationAsync(params string[] scopes)
        {
            return await _service.GetAccessTokenForApplicationAsync(scopes);
        }

        public async Task<AccessTokenCredential> GetAccessTokenOnBehalfOfCurrentUserAsync(string resource = "https://management.core.windows.net")
        {
            return await _service.GetAccessTokenOnBehalfOfCurrentUserAsync(resource);
        }
    }
}
