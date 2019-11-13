using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIPAccessDeniedAlerts
{
    public class AccessToken
    {
        static ClientCredential credential;
        static AuthenticationContext authContext;

        public AccessToken()
        {

        }

        public async Task<AuthenticationResult> GetToken(string tenantId, string appId, string appSecret)
        {
            authContext = new AuthenticationContext("https://login.microsoftonline.com/" + tenantId);

            credential = new ClientCredential(appId, appSecret);

            var GraphAAD_URL = string.Format("https://graph.microsoft.com/");

            try
            {
                AuthenticationResult result = await authContext.AcquireTokenAsync(GraphAAD_URL, credential);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Acquiring Access Token: \n" + ex.Message);
            }
        }
    }
}
