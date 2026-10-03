using Microsoft.PowerPlatform.Dataverse.Client;

namespace XrmToolsLearning;

internal static class DataverseConnection
{
    internal const string DefaultEnvironmentUrl = "https://******.crm.dynamics.com/";
    internal const string DefaultUserName = "******@******.onmicrosoft.com";

    // .NET system-browser authentication requires the sample app's loopback redirect.
    private const string SampleApplicationId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
    private const string SampleRedirectUri = "http://localhost";

    internal static ServiceClient Connect()
    {
        string environmentUrl =
            Environment.GetEnvironmentVariable("DATAVERSE_URL") ?? DefaultEnvironmentUrl;
        string userName =
            Environment.GetEnvironmentVariable("DATAVERSE_USERNAME") ?? DefaultUserName;
        string tokenCachePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "XrmToolsLearning",
            "msal_cache.data");

        Directory.CreateDirectory(Path.GetDirectoryName(tokenCachePath)!);

        string connectionString = string.Join(
            ';',
            "AuthType=OAuth",
            $"Url={environmentUrl}",
            $"Username={userName}",
            $"AppId={SampleApplicationId}",
            $"RedirectUri={SampleRedirectUri}",
            $"TokenCacheStorePath={tokenCachePath}",
            "LoginPrompt=Auto");

        var service = new ServiceClient(connectionString);
        if (!service.IsReady)
        {
            string detail = service.LastException?.ToString() ?? service.LastError;
            service.Dispose();
            throw new InvalidOperationException(
                $"Unable to connect to {environmentUrl} as {userName}.{Environment.NewLine}{detail}");
        }

        return service;
    }
}
