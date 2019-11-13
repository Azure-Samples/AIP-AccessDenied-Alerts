using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Azure.OperationalInsights;
using Microsoft.Azure.OperationalInsights.Models;
using System.Net.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace AIPAccessDeniedAlerts
{
    public static class Function1
    {

        private static HttpClient client = new HttpClient();

        // Get WorkspaceID, ClientID and Client Secret from Azure KeyVault
        private static string workspaceId = System.Environment.GetEnvironmentVariable("LAWorkSpaceIDFromAKV");
        private static string clientId = System.Environment.GetEnvironmentVariable("ClientIDFromAKV");
        private static string clientSecret = System.Environment.GetEnvironmentVariable("ClientSecretFromAKV");
        private static string tenantId = System.Environment.GetEnvironmentVariable("AzTenantIdFromAKV");
        private static string domain = System.Environment.GetEnvironmentVariable("AzDomainNameFromAKV");
        private static string SenderEmail = System.Environment.GetEnvironmentVariable("SenderEmailFromAKV");
        private static string appUri = System.Environment.GetEnvironmentVariable("appUriFromAKV");
        private static AIPFile.Result accessActivity = null;

        // Configure app builder for the Microsoft Graph AccessToken
        //private static string authority = $"https://login.microsoftonline.com/{tenantId}";

        [FunctionName("Function1")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            // Step 1: Get Graph Token
            var token = await GetToken();

            //log.LogInformation($"MY TOKEN: {token.AccessToken}");

            // Step 2: Get AIP Access Denied Activity

            var accessDeniedActivity = new GetFilesActivity();
            Table activityResults = await accessDeniedActivity.RunLAQuery(domain, clientId, clientSecret, workspaceId);

            var jsonObj = JsonConvert.SerializeObject(activityResults);

            var AIPQueryResults = JsonConvert.DeserializeObject<AIPFile.Table>(jsonObj);

            // Iterate through each row and email each user

            for (int i = 0; i < activityResults.Rows.Count; i++)
            {
                accessActivity = new AIPFile.Result
                {
                    ContentId_g = activityResults.Rows[i][0],
                    FileName = activityResults.Rows[i][1],
                    LabelName_s = activityResults.Rows[i][2],
                    UserId_s = activityResults.Rows[i][3],
                    ProtectionOwner_s = activityResults.Rows[i][4],
                    TimeGenerated = activityResults.Rows[i][5],
                    ProtectionTime_t = activityResults.Rows[i][6],
                    IPv4_s = activityResults.Rows[i][7],
                    Activity_s = activityResults.Rows[i][8],
                    Operation_s = activityResults.Rows[i][9],
                    AccessCount = activityResults.Rows[i][10]
                };

                // Create New Email Message using Graph
                var NewMessage = new Message
                    {
                        Subject = "AIP File Access Denied Alert",
                        Body = new ItemBody
                        {
                            ContentType = BodyType.Html,
                            Content = "<H2>A user was denied access to sensitive file you protected.</H2>" +
                            "<table>" +
                            "<tr>" +
                            "<td>" +
                            "User Name: " + "<b>" + accessActivity.UserId_s + "<b/>" +
                            "</td>" +
                            "<tr/>" +
                            "<tr>" +
                            "<td>" +
                            "File Name: " + "<b>" + accessActivity.FileName + "<b/>" +
                            "</td>" +
                            "<tr/>" +
                            "<tr>" +
                            "<td>" +
                            "Label Name: " + "<b>" + accessActivity.LabelName_s + "<b/>" +
                            "</td>" +
                            "<tr/>" +
                            "<tr>" +
                            "<td>" +
                            "Access Denied Date: " + "<b>" + DateTime.SpecifyKind(DateTime.Parse(accessActivity.TimeGenerated), DateTimeKind.Utc) + "<b/>" +
                            "</td>" +
                            "<tr/>" +
                            "<tr>" +
                            "<td>" +
                            "Protection Date: " + "<b>" + DateTime.SpecifyKind(DateTime.Parse(accessActivity.ProtectionTime_t), DateTimeKind.Utc) + "<b/>" +
                            "</td>" +
                            "<tr/>" +
                            "<tr>" +
                            "<td>" +
                            "Number of Attempts: " + "<b>" + accessActivity.AccessCount + "<b/>" +
                            "</td>" +
                            "<tr/>" +
                            "</table>"
                        },

                        ToRecipients = new List<Recipient>()
                        {
                            new Recipient
                            {
                                EmailAddress = new EmailAddress
                                {
                                    Address = accessActivity.ProtectionOwner_s
                                }
                            }
                        },
                };

                var AlertAIPUser = new SendEmailController();
                var myAlertResult = await AlertAIPUser.SendAIPEmailAlert(NewMessage, SenderEmail, token.AccessToken);

                ////// ************** Send securityAction to Microsoft Security Graph API *************
                List<String> AIPEvent = new List<string>();
            AIPEvent.Add(accessActivity.ContentId_g);
            AIPEvent.Add(accessActivity.FileName);
            AIPEvent.Add(accessActivity.LabelName_s);
            AIPEvent.Add(accessActivity.UserId_s);
            AIPEvent.Add(accessActivity.ProtectionOwner_s);
            AIPEvent.Add(accessActivity.TimeGenerated);
            AIPEvent.Add(accessActivity.ProtectionTime_t);
            AIPEvent.Add(accessActivity.IPv4_s);
            AIPEvent.Add(accessActivity.Activity_s);
            AIPEvent.Add(accessActivity.AccessCount);

            // BUILD NEW TI FOR AZURE SENTINEL
            var newTIIndicator = new TiIndicator
            {
                Action = TiAction.Alert,
                Confidence = 0,
                Description = "AIP Access Denied Alert.",
                ExpirationDateTime = DateTimeOffset.UtcNow.AddDays(7),
                FileName = accessActivity.FileName,
                NetworkIPv4 = accessActivity.IPv4_s,
                ExternalId = accessActivity.ContentId_g,
                Severity = 2,
                Tags = AIPEvent,
                TargetProduct = "Azure Sentinel",
                ThreatType = "WatchList",
                TlpLevel = TlpLevel.Green
            };

                // Send TI to Azure Sentinel via Microsoft Inelligent Security Graph
                try
            {
                var AzSentinelTI = new TIIndicatorsController();

                string MyTI = await AzSentinelTI.CreateTI(newTIIndicator, token.AccessToken);
                log.LogInformation($"TI RESULTS: {MyTI}");

            }
            catch (Exception e)
            {
                log.LogInformation($"AIP TiIndicator ERROR: {e}");
            }
        }

        }

        private static async Task<AuthenticationResult> GetToken()
        {
            var accessToken = new AccessToken();

            var token = await accessToken.GetToken(tenantId, clientId, clientSecret);

            return token;
        }
    }
}
