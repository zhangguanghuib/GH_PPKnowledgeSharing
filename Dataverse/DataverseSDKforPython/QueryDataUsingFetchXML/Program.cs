using System.Globalization;
using System.Xml.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Identity.Client;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

internal static class Program
{
	private const string DefaultEnvironmentUrl = "https://org5efadec2.crm.dynamics.com/";
	private const string DefaultUserName = "guazha@dynamicsFTEGCR.onmicrosoft.com";
	private const string SampleClientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";

	private static int Main(string[] args)
	{
		// string command = args.FirstOrDefault()?.ToLowerInvariant() ?? "basic";
        string command = "filter";
		if (command is "help" or "--help" or "-h")
		{
			ShowHelp();
			return 0;
		}

		try
		{
			Console.WriteLine("Connecting to Dataverse...");
			using ServiceClient service = Connect();
			Console.WriteLine("Connection initialized. Verifying identity...");
			WhoAmIResponse identity = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
			Console.WriteLine($"Connected to {service.ConnectedOrgFriendlyName}");
			Console.WriteLine($"User ID: {identity.UserId}\n");

			switch (command)
			{
				case "basic":
					ExecuteAndPrint(service, BasicQuery(), "Basic selection and ordering");
					break;
				case "filter":
					ExecuteAndPrint(service, FilterQuery(args.ElementAtOrDefault(1) ?? "%"), "Filtered accounts");
					break;
				case "join":
					ExecuteAndPrint(service, JoinQuery(), "Accounts joined to primary contacts");
					break;
				case "aggregate":
					ExecuteAndPrint(service, AggregateQuery(), "Accounts grouped by status");
					break;
				case "count":
					ExecuteCount(service);
					break;
				case "paged":
					ExecutePaged(service, ParsePositiveInt(args, 1, 3), ParseNonNegativeInt(args, 2, 2));
					break;
				case "custom":
					ExecuteCustom(service, args.ElementAtOrDefault(1));
					break;
				default:
					Console.Error.WriteLine($"Unknown command: {command}\n");
					ShowHelp();
					return 2;
			}

			return 0;
		}
		catch (Exception exception)
		{
			Console.WriteLine($"Error: {exception}");
			return 1;
		}
	}

	private static ServiceClient Connect()
	{
		string environmentUrl = Environment.GetEnvironmentVariable("DATAVERSE_URL") ?? DefaultEnvironmentUrl;
		string userName = Environment.GetEnvironmentVariable("DATAVERSE_USER") ?? DefaultUserName;
		string clientId = Environment.GetEnvironmentVariable("DATAVERSE_CLIENT_ID") ?? SampleClientId;

		IPublicClientApplication application = PublicClientApplicationBuilder
			.Create(clientId)
			.WithAuthority("https://login.microsoftonline.com/organizations")
			.Build();

		ServiceClient service = new(
			new Uri(environmentUrl),
			instanceUrl => AcquireTokenAsync(application, instanceUrl, userName),
			useUniqueInstance: true);
		if (!service.IsReady)
		{
			string details = service.LastException?.Message ?? service.LastError;
			service.Dispose();
			throw new InvalidOperationException($"Dataverse sign-in failed. {details}");
		}

		return service;
	}

	private static async Task<string> AcquireTokenAsync(
		IPublicClientApplication application,
		string instanceUrl,
		string userName)
	{
		string scope = $"{new Uri(instanceUrl).GetLeftPart(UriPartial.Authority)}/.default";
		IAccount? account = (await application.GetAccountsAsync()).FirstOrDefault();

		if (account is not null)
		{
			try
			{
				AuthenticationResult cached = await application
					.AcquireTokenSilent([scope], account)
					.ExecuteAsync();
				return cached.AccessToken;
			}
			catch (MsalUiRequiredException)
			{
			}
		}

		AuthenticationResult result = await application
			.AcquireTokenWithDeviceCode([scope], deviceCode =>
			{
				Console.WriteLine("Microsoft sign-in is required.");
				Console.WriteLine($"Open: {deviceCode.VerificationUrl}");
				Console.WriteLine($"Code: {deviceCode.UserCode}");
				Console.WriteLine($"Account: {userName}\n");
				return Task.CompletedTask;
			})
			.ExecuteAsync();

		Console.WriteLine("Microsoft sign-in completed. Initializing the Dataverse client...");
		return result.AccessToken;
	}

	private static string BasicQuery() => """
		<fetch top='5'>
		  <entity name='account'>
			<attribute name='accountid' />
			<attribute name='name' />
			<attribute name='createdon' />
			<order attribute='name' />
		  </entity>
		</fetch>
		""";

	private static string FilterQuery(string searchText)
	{
		string pattern = searchText.Contains('%') ? searchText : $"%{searchText}%";
		XElement fetch = new("fetch", new XAttribute("top", 10),
			new XElement("entity", new XAttribute("name", "account"),
				new XElement("attribute", new XAttribute("name", "accountid")),
				new XElement("attribute", new XAttribute("name", "name")),
				new XElement("attribute", new XAttribute("name", "address1_city")),
				new XElement("filter", new XAttribute("type", "and"),
					new XElement("condition",
						new XAttribute("attribute", "name"),
						new XAttribute("operator", "like"),
						new XAttribute("value", pattern))),
				new XElement("order", new XAttribute("attribute", "name"))));

		return fetch.ToString(SaveOptions.DisableFormatting);
	}

	private static string JoinQuery() => """
		<fetch top='10'>
		  <entity name='account'>
			<attribute name='accountid' />
			<attribute name='name' />
			<order attribute='name' />
			<link-entity name='contact' from='contactid' to='primarycontactid'
						 link-type='outer' alias='primarycontact'>
			  <attribute name='fullname' alias='contact_name' />
			  <attribute name='emailaddress1' alias='contact_email' />
			</link-entity>
		  </entity>
		</fetch>
		""";

	private static string AggregateQuery() => """
		<fetch aggregate='true'>
		  <entity name='account'>
			<attribute name='statecode' alias='status' groupby='true' />
			<attribute name='accountid' alias='account_count' aggregate='count' />
			<order alias='status' />
		  </entity>
		</fetch>
		""";

	private static void ExecuteCount(IOrganizationService service)
	{
		const string fetchXml = """
			<fetch count='10' page='1' returntotalrecordcount='true'>
			  <entity name='account'>
				<attribute name='accountid' />
				<attribute name='name' />
				<order attribute='name' />
			  </entity>
			</fetch>
			""";

		EntityCollection results = service.RetrieveMultiple(new FetchExpression(fetchXml));
		PrintResults(results, "Count with first page");
		string suffix = results.TotalRecordCountLimitExceeded ? "+ (limit exceeded)" : string.Empty;
		Console.WriteLine($"Total matching rows: {results.TotalRecordCount}{suffix}");
	}

	private static void ExecutePaged(IOrganizationService service, int pageSize, int maxPages)
	{
		XElement fetch = XElement.Parse("""
			<fetch>
			  <entity name='account'>
				<attribute name='accountid' />
				<attribute name='name' />
				<order attribute='name' />
				<order attribute='accountid' />
			  </entity>
			</fetch>
			""");
		fetch.SetAttributeValue("count", pageSize);

		int pageNumber = 1;
		int totalRows = 0;
		while (maxPages == 0 || pageNumber <= maxPages)
		{
			fetch.SetAttributeValue("page", pageNumber);
			EntityCollection page = service.RetrieveMultiple(new FetchExpression(fetch.ToString(SaveOptions.DisableFormatting)));
			PrintResults(page, $"Page {pageNumber}");
			totalRows += page.Entities.Count;

			if (!page.MoreRecords)
			{
				break;
			}

			fetch.SetAttributeValue("paging-cookie", page.PagingCookie);
			pageNumber++;
		}

		Console.WriteLine($"Rows retrieved: {totalRows}");
		if (maxPages > 0 && pageNumber >= maxPages)
		{
			Console.WriteLine("Paging stopped at the requested page limit. Use 0 for all pages.");
		}
	}

	private static void ExecuteCustom(IOrganizationService service, string? path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ArgumentException("The custom command requires a path to a FetchXML file.");
		}

		ExecuteAndPrint(service, File.ReadAllText(path), $"Custom query: {path}");
	}

	private static void ExecuteAndPrint(IOrganizationService service, string fetchXml, string title)
	{
		EntityCollection results = service.RetrieveMultiple(new FetchExpression(fetchXml));
		PrintResults(results, title);
	}

	private static void PrintResults(EntityCollection results, string title)
	{
		Console.WriteLine($"== {title} ==");
		Console.WriteLine($"Returned {results.Entities.Count} row(s). More records: {results.MoreRecords}");

		foreach (Entity entity in results.Entities)
		{
			Console.WriteLine($"\n{entity.LogicalName} {entity.Id}");
			foreach ((string name, object value) in entity.Attributes.OrderBy(attribute => attribute.Key))
			{
				string displayValue = entity.FormattedValues.TryGetValue(name, out string? formatted)
					? formatted
					: FormatValue(value);
				Console.WriteLine($"  {name}: {displayValue}");
			}
		}

		Console.WriteLine();
	}

	private static string FormatValue(object? value) => value switch
	{
		null => "<null>",
		AliasedValue aliased => FormatValue(aliased.Value),
		EntityReference reference => reference.Name ?? $"{reference.LogicalName}:{reference.Id}",
		Money money => money.Value.ToString(CultureInfo.InvariantCulture),
		OptionSetValue option => option.Value.ToString(CultureInfo.InvariantCulture),
		DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
		IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
		_ => value.ToString() ?? string.Empty
	};

	private static int ParsePositiveInt(string[] args, int index, int defaultValue)
	{
		if (args.Length <= index)
		{
			return defaultValue;
		}

		return int.TryParse(args[index], out int value) && value > 0
			? value
			: throw new ArgumentException($"Argument {index + 1} must be a positive integer.");
	}

	private static int ParseNonNegativeInt(string[] args, int index, int defaultValue)
	{
		if (args.Length <= index)
		{
			return defaultValue;
		}

		return int.TryParse(args[index], out int value) && value >= 0
			? value
			: throw new ArgumentException($"Argument {index + 1} must be zero or a positive integer.");
	}

	private static void ShowHelp()
	{
		Console.WriteLine("""
			Query Data Using FetchXML

			Usage:
			  dotnet run -- [command] [arguments]

			Commands:
			  basic                   Select and order five accounts (default)
			  filter [text]           Find up to ten account names containing text
			  join                    Outer join accounts to primary contacts
			  aggregate               Group accounts by status and count them
			  count                   Return the first page and total record count
			  paged [size] [pages]    Page accounts; pages=0 retrieves every page
			  custom <file>           Execute FetchXML loaded from a file
			  help                    Show this help

			Optional environment variables:
			  DATAVERSE_URL
			  DATAVERSE_USER
			  DATAVERSE_CLIENT_ID
			""");
	}
}
