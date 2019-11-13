---
page_type: sample
languages:
- csharp
products:
- dotnet
description: "This sample solution illustrates how to send AIP access denied alerts to users and Azure Sentinel via the Graph Security API "
urlFragment: "AIP-AccessDenied-Alerts"
---

# AIP Notification Function for Security and Compliance Professionals

<!-- 
Guidelines on README format: https://review.docs.microsoft.com/help/onboard/admin/samples/concepts/readme-template?branch=master

Guidance on onboarding samples to docs.microsoft.com/samples: https://review.docs.microsoft.com/help/onboard/admin/samples/process/onboarding?branch=master

Taxonomies for products and languages: https://review.docs.microsoft.com/new-hope/information-architecture/metadata/taxonomies?branch=master
-->

Azure Information Protection Custom Notification Solution for Users and Administrators.

## Summary
This repo contains sample code demonstrating how you can build a custom solution that helps your organizations notify AIP document owners when an access denied event is received for documents they protected. The sample solution uses an Azure Function to query Log Analytics and then sends each document owner an email via the Microsoft Graph. Optionally, the solution also shows how to send an alert to Azure Sentinel via Microsoft Graph Security API.  

Why should you build something like this? Some customers have expressed the desired to notify document owners when a file access attempt is received for a document they protected using AIP. 

Please read our first blog [for more details on how to configure some of these settings](https://techcommunity.microsoft.com/t5/Azure-Information-Protection/How-to-Build-a-Custom-AIP-Tracking-Portal/ba-p/875849 "How to Build a Custom AIP Tracking Portal")

## Prerequisites

While the solution is quite simple, some assembly is required.

•	Visual Studio 2017 or higher  
•	An Azure subscription with a Log Analytics Workspace created  
•	Azure Information Protection (AIP) with Log Analytics integration configured   
•	Either Classic or Unified Labeling client installed on a supported version of Windows (7 or above as of today)   
•	One Azure Function   
•	An Azure AD application (Service Principal)  
•	Optional: Azure Key Vault  


## Setup

## Clone the Repository
1. Open a command prompt  
2. Create a new folder mkdir c:\samples  
3. Navigate to the new folder using cd c:\samples  
4. Clone the repository by running git clone https://github.com/Azure-Samples/AIP-AccessDenied-Alerts  
5. In explorer, navigate to c:\samples\AIP-AccessDenied-Alerts and open the AIP-AccessDenied-Alerts.sln in Visual Studio 2017 or later.  

## Add the NuGet Package
In Visual Studio, right click the _AIP-AccessDenied-Alerts_ solution.  
Click **Restore NuGet Packages**

## Authentication
This sample solution uses a single application (service principal) that you must register in Azure AD. Note that this service pricipal requires **Data.Reader** rights in your Log Analytics Workspace as explained on the blog above.  
[Follow these instructions to register an application in Azure Active Directory](https://dev.loganalytics.io/oms/documentation/1-Tutorials/1-Direct-API "Register Azure AD app")

## Azure Functions keys
To view your keys, create new ones, navigate to one of your HTTP-triggered functions in the [Azure portal]( https://portal.azure.com "Azure Portal") and select **Manage**.  
Functions lets you use keys to make it harder to access your HTTP function endpoints during development. A standard HTTP trigger may require such an API key be present in the request. Most HTTP trigger templates require an API key in the request. 
 
 ## Important
While keys may help obfuscate your HTTP endpoints during development, they are not intended to secure an HTTP trigger in production. To learn more, see [Secure an HTTP endpoint in production](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook#secure-an-http-endpoint-in-production "Secure HTTP endpoint in production").

## Setup/Configure Azure Key Vault
Although Azure Key Vault is an optional component, we highly recommend it. As a bonus, it’s already wired up on both Azure Functions.    

**NOTE** Make sure you follow the [managed identities]( https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity#creating-an-app-with-an-identity "Tenant ID") instructions as well if you decide to use Key Vault.

Please follow Jeff’s excellent walkthrough on how to setup Key Vault: [Configure Azure Key Vault]( https://medium.com/statuscode/getting-key-vault-secrets-in-azure-functions-37620fd20a0b "Tenant ID")

## Update appSettings
For production deployment you may want to use the Azure Key Vault implementation to make sure your keys/secrets are properly protected. For testing however, you can just assign hardcoded values to the variables below within the Function1 Run method.

| Key       | Value                                |
|-------------------|--------------------------------------------|
| `workspaceId`             | Log Analytics Worskpace ID from the [Log Analytics workspaces blade]( https://portal.azure.com/#blade/HubsExtension/BrowseResourceBlade/resourceType/Microsoft.OperationalInsights%2Fworkspaces "Workspace Id") |
| `clientId`      | From the [AAD Registered Apps blade]( https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps "Client Id")      |
| `clientSecret`    | From the [AAD Registered Apps blade]( https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps "Client Secret")            |
| `tenantId`      | From the [AAD Properties Blade]( https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Properties "Tenant ID")  |
| `domain`    | Domain of AAD Tenant - e.g. Contoso.Onmicrosoft.com      |
| `SenderEmail`    | Sender Email Address - e.g. userid@domain.com      |
| `appUri`    | App URI from AAD App registration     |

## Publish your Function to Azure
You can just right click on the _AIP-AccessDenied-Alerts_ solution in Visual Studio and click Publish.  
Please follow these instructions on [how to publish an Azure Function to Azure using Visual Studio]( https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio "Publish First Function to Azure"). 

Finally, we want to hear from you. Please contribute and let us know what other use cases you come up with.

## Sources/Attribution/License/3rd Party Code
Unless otherwise noted, all content is licensed under MIT license.  
JSON de/serialization provided by [Json.NET](https://www.nuget.org/packages/Newtonsoft.Json/)  

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
