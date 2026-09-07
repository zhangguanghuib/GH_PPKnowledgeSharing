using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PowerApps.Samples
{
    /// <summary>
    /// Demonstrates Azure authentication and execution of a Dataverse Web API function.
    /// </summary>
    class Program
    {
        static async Task Main()
        {
            string resource = "https://org5efadec2.crm.dynamics.com";

            // Microsoft Entra ID app registration shared by all Power Apps samples.
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            var redirectUri = "http://localhost";

            #region Authentication

            var authBuilder = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                .WithRedirectUri(redirectUri)
                .Build();
            var scope = resource + "/user_impersonation";
            string[] scopes = { scope };

            AuthenticationResult token =
                await authBuilder.AcquireTokenInteractive(scopes).ExecuteAsync();

            #endregion Authentication

            #region Client configuration

            using var client = new HttpClient
            {
                BaseAddress = new Uri(resource + "/api/data/v9.2/"),
                Timeout = new TimeSpan(0, 2, 0)
            };

            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            #endregion Client configuration

            #region Web API call

            using var response = await client.GetAsync("WhoAmI");

            if (response.IsSuccessStatusCode)
            {
                string jsonContent = await response.Content.ReadAsStringAsync();

                using JsonDocument document = JsonDocument.Parse(jsonContent);
                Guid userId = document.RootElement.GetProperty("UserId").GetGuid();

                Console.WriteLine($"Your user ID is {userId}");
            }
            else
            {
                Console.WriteLine("Web API call failed");
                Console.WriteLine("Reason: " + response.ReasonPhrase);
            }

            #endregion Web API call
        }
    }

    /// <summary>
    /// Represents the response returned by the Dataverse WhoAmI function.
    /// </summary>
    public class WhoAmIResponse
    {
        public Guid BusinessUnitId { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
