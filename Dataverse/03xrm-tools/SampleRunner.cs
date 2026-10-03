using Microsoft.PowerPlatform.Dataverse.Client;

namespace XrmToolsLearning;

internal static class SampleRunner
{
    private static readonly string[] SampleOrder =
        ["connection", "retrieve", "crud", "messages", "association", "webapi", "parallel"];

    private static readonly IReadOnlyDictionary<string, (string Description, Func<ServiceClient, Task> Run)>
        Samples = new Dictionary<string, (string, Func<ServiceClient, Task>)>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["connection"] = ("Display connection, user, and Dataverse version information", SampleOperations.ConnectionInfoAsync),
            ["retrieve"] = ("Run FetchXML and retrieve an account by ID", SampleOperations.RetrieveAsync),
            ["crud"] = ("Create, annotate, retrieve, update, change state, and delete an account", SampleOperations.CrudAsync),
            ["messages"] = ("Run CreateRequest and RetrieveMultipleRequest", SampleOperations.MessagesAsync),
            ["association"] = ("Create and remove an account-to-lead association", SampleOperations.AssociationAsync),
            ["webapi"] = ("Create and delete an account through ExecuteWebRequest", SampleOperations.WebApiAsync),
            ["parallel"] = ("Create and delete ten accounts with cloned clients and TPL", SampleOperations.ParallelAsync),
        };

    internal static async Task<int> RunAsync(string[] args)
    {
        if (args.Length > 0 && args[0] is "--help" or "-h")
        {
            PrintHelp();
            return 0;
        }

        if (args.Length > 0 && args[0] is "--list")
        {
            PrintSamples();
            return 0;
        }

        string sampleName = args.Length == 0
            ? "all"
            : args[0] == "--sample" && args.Length > 1 ? args[1] : args[0];
        bool runAll = sampleName.Equals("all", StringComparison.OrdinalIgnoreCase);
        if (!runAll && !Samples.ContainsKey(sampleName))
        {
            Console.Error.WriteLine($"Unknown sample: {sampleName}");
            PrintSamples();
            return 2;
        }

        try
        {
            string[] selectedSamples = runAll ? SampleOrder : [sampleName];
            if (runAll)
            {
                Console.WriteLine(
                    "Running all samples, including temporary Dataverse record creation and deletion.");
            }

            Console.WriteLine(
                $"Connecting to {Environment.GetEnvironmentVariable("DATAVERSE_URL") ?? DataverseConnection.DefaultEnvironmentUrl}");
            using ServiceClient service = DataverseConnection.Connect();
            foreach (string name in selectedSamples)
            {
                var sample = Samples[name];
                Console.WriteLine($"\n=== {name}: {sample.Description} ===");
                await sample.Run(service);
                Console.WriteLine($"Sample '{name}' completed successfully.");
            }

            Console.WriteLine("All selected samples completed successfully.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine(
            """
            XRM Tooling / Dataverse learning samples

            Usage:
              dotnet run
              dotnet run -- all
              dotnet run -- --list
              dotnet run -- --sample <name>
              dotnet run -- <name>

            Authentication opens Microsoft's interactive sign-in UI. No password is stored.
            Override the configured environment with DATAVERSE_URL and DATAVERSE_USERNAME.
            Write samples create temporary records and clean them up before returning.
            No arguments or 'all' runs every sample in sequence and stops on the first error.
            """);
        PrintSamples();
    }

    private static void PrintSamples()
    {
        Console.WriteLine("Samples:");
        Console.WriteLine("  all          Run every sample in sequence (default; includes writes)");
        foreach (string name in SampleOrder)
        {
            string description = Samples[name].Description;
            Console.WriteLine($"  {name,-12} {description}");
        }
    }
}
