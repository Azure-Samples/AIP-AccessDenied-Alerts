using Microsoft.Azure.OperationalInsights;
using Microsoft.Azure.OperationalInsights.Models;
using Microsoft.Graph;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIPAccessDeniedAlerts
{
    class GetFilesActivity
    {

            public async Task<Table> RunLAQuery(string domain, string clientId, string clientSecret, string workspaceId)
            {

                var authEndpoint = "https://login.microsoftonline.com";
                var tokenAudience = "https://api.loganalytics.io/";

                var adSettings = new ActiveDirectoryServiceSettings
                {
                    AuthenticationEndpoint = new Uri(authEndpoint),
                    TokenAudience = new Uri(tokenAudience),
                    ValidateAuthority = true
                };

                var creds = ApplicationTokenProvider.LoginSilentAsync(domain, clientId, clientSecret, adSettings).GetAwaiter().GetResult();
                var LAclient = new OperationalInsightsDataClient(creds)
                {
                    WorkspaceId = workspaceId
                };

            // Log Analytics Kusto query - look for user data in the past 24 hours
            string query = @"
                let lookback = timespan(24h);
                let doclookup = InformationProtectionLogs_CL
                | where ContentId_g != '' and ObjectId_s != ''
                    and TimeGenerated >= ago(90d) 
                | distinct ContentId_g, ObjectId_s;
                let accesslookup = InformationProtectionLogs_CL
                | where TimeGenerated >= ago(lookback)  
                | where Activity_s  == 'AccessDenied'
                | extend AccessCount = 1;
                    accesslookup
                | join kind = inner(
                    doclookup
                ) on $left.ContentId_g == $right.ContentId_g
                | extend FileName = extract('((([^\\/\\\\]*\\.[a-z]{1,4}$))|([[^\\/\\\\]*$))', 1, ObjectId_s1)
                | summarize AccessCount = sum(AccessCount) by ContentId_g, FileName, LabelName_s, UserId_s, ProtectionOwner_s, 
                    TimeGenerated, ProtectionTime_t, IPv4_s, Activity_s, Operation_s";

                var outputTable = await LAclient.QueryAsync(query.Trim());

                return outputTable.Tables[0];
            }
        }

        }
