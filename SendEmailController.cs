using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AIPAccessDeniedAlerts
{
    class SendEmailController
    {

        #region Send email notification
        public async Task<string> SendAIPEmailAlert(Message EmailITem, String SenderEmail, string token)
        {
            //var token = await GetToken();

            string url = string.Format($"https://graph.microsoft.com/beta/users/{SenderEmail}/sendMail");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var stringEmailContent = JsonConvert.SerializeObject(EmailITem);

            request.Content = new StringContent(stringEmailContent, Encoding.UTF8, "application/json");

            HttpClient http = new HttpClient();

            var response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                return JsonConvert.SerializeObject(formatted, Formatting.Indented);
            }

            string json = await response.Content.ReadAsStringAsync();

            return json;
        }
        #endregion

    }
}
