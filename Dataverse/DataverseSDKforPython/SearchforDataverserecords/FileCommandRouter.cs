using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

internal static class FileCommandRouter
{
    public static readonly HashSet<string> Commands = new(StringComparer.OrdinalIgnoreCase)
    {
        "file-max", "file-info", "file-related", "file-upload", "file-download", "file-delete",
        "image-max", "image-full-columns", "image-primary-columns", "image-thumbnail",
        "image-upload", "image-update", "image-delete",
        "attachment-upload", "attachment-upload-direct", "attachment-download",
        "note-upload", "note-upload-direct", "note-download", "attachment-max",
        "file-sas-url"
    };

    public static void Execute(IOrganizationService service, string[] args)
    {
        string command = args[0].ToLowerInvariant();
        switch (command)
        {
            case "file-max":
                RequireArguments(args, 3, "file-max <table> <file-column>");
                Console.WriteLine($"Maximum file size: {DataverseFileOperations.GetFileColumnMaxSizeInKb(service, args[1], args[2]):N0} KB");
                break;
            case "file-info":
                RequireArguments(args, 4, "file-info <table> <guid> <file-column>");
                PrintEntity(DataverseFileOperations.RetrieveRecordWithFileColumns(
                    service, args[1], ParseGuid(args[2]), args[3], $"{args[3]}_name"));
                break;
            case "file-related":
                RequireArguments(args, 3, "file-related <table> <guid>");
                PrintEntities(DataverseFileOperations.GetRelatedFileInformation(service, args[1], ParseGuid(args[2])));
                break;
            case "file-upload":
            case "image-upload":
                RequireConfirmed(args, 5, $"{command} <table> <guid> <column> <path> --confirm");
                Guid fileId = DataverseFileOperations.UploadFile(
                    service, new EntityReference(args[1], ParseGuid(args[2])), args[3], new FileInfo(args[4]));
                Console.WriteLine($"Uploaded file ID: {fileId}");
                break;
            case "file-download":
                RequireArguments(args, 5, "file-download <table> <guid> <column> <output-path>");
                WriteDownload(
                    DataverseFileOperations.DownloadFile(service, new EntityReference(args[1], ParseGuid(args[2])), args[3]),
                    args[4]);
                break;
            case "file-delete":
                RequireConfirmed(args, 2, "file-delete <file-guid> --confirm");
                DataverseFileOperations.DeleteFile(service, ParseGuid(args[1]));
                Console.WriteLine("File deleted.");
                break;
            case "image-max":
                RequireArguments(args, 3, "image-max <table> <image-column>");
                Console.WriteLine($"Maximum image size: {DataverseFileOperations.GetImageColumnMaxSizeInKb(service, args[1], args[2]):N0} KB");
                break;
            case "image-full-columns":
                DataverseFileOperations.PrintFullSizedImageColumns(service);
                break;
            case "image-primary-columns":
                DataverseFileOperations.PrintPrimaryImageColumns(service);
                break;
            case "image-thumbnail":
                RequireArguments(args, 5, "image-thumbnail <table> <guid> <column> <output-path>");
                WriteBytes(DataverseFileOperations.RetrieveThumbnail(service, args[1], ParseGuid(args[2]), args[3]), args[4]);
                break;
            case "image-update":
                RequireConfirmed(args, 5, "image-update <table> <guid> <column> <path> --confirm");
                DataverseFileOperations.UpdateImage(
                    service, args[1], ParseGuid(args[2]), args[3], File.ReadAllBytes(Path.GetFullPath(args[4])));
                Console.WriteLine("Image updated.");
                break;
            case "image-delete":
                RequireConfirmed(args, 4, "image-delete <table> <guid> <column> --confirm");
                DataverseFileOperations.DeleteImage(service, args[1], ParseGuid(args[2]), args[3]);
                Console.WriteLine("Image deleted.");
                break;
            case "attachment-upload":
                RequireConfirmed(args, 3, "attachment-upload <email-guid> <path> --confirm");
                Entity attachment = CreateAttachment(ParseGuid(args[1]), new FileInfo(args[2]));
                CommitAttachmentBlocksUploadResponse attachmentResponse =
                    DataverseFileOperations.UploadAttachment(service, attachment, new FileInfo(args[2]));
                Console.WriteLine($"Attachment ID: {attachmentResponse.ActivityMimeAttachmentId}; bytes: {attachmentResponse.FileSizeInBytes:N0}");
                break;
            case "attachment-upload-direct":
                RequireConfirmed(args, 3, "attachment-upload-direct <email-guid> <path> --confirm");
                Console.WriteLine($"Attachment ID: {DataverseFileOperations.CreateAttachmentDirect(service, ParseGuid(args[1]), new FileInfo(args[2]))}");
                break;
            case "attachment-download":
                RequireArguments(args, 3, "attachment-download <attachment-guid> <output-path>");
                WriteDownload(DataverseFileOperations.DownloadAttachment(
                    service, new EntityReference("activitymimeattachment", ParseGuid(args[1]))), args[2]);
                break;
            case "note-upload":
                RequireConfirmed(args, 4, "note-upload <regarding-table> <regarding-guid> <path> --confirm");
                Entity note = CreateNote(args[1], ParseGuid(args[2]), new FileInfo(args[3]));
                CommitAnnotationBlocksUploadResponse noteResponse =
                    DataverseFileOperations.UploadNote(service, note, new FileInfo(args[3]));
                Console.WriteLine($"Note ID: {noteResponse.AnnotationId}; bytes: {noteResponse.FileSizeInBytes:N0}");
                break;
            case "note-upload-direct":
                RequireConfirmed(args, 4, "note-upload-direct <regarding-table> <regarding-guid> <path> --confirm");
                Console.WriteLine($"Note ID: {DataverseFileOperations.CreateNoteDirect(
                    service, new EntityReference(args[1], ParseGuid(args[2])), new FileInfo(args[3]))}");
                break;
            case "note-download":
                RequireArguments(args, 3, "note-download <note-guid> <output-path>");
                WriteDownload(DataverseFileOperations.DownloadNote(
                    service, new EntityReference("annotation", ParseGuid(args[1]))), args[2]);
                break;
            case "attachment-max":
                Console.WriteLine($"Maximum attachment/note upload size: {DataverseFileOperations.GetMaxUploadFileSize(service):N0} bytes");
                break;
            case "file-sas-url":
                RequireConfirmed(args, 3, "file-sas-url <table> <guid> [column] [retained|bin] --confirm");
                string table = args[1];
                string? column = GetOptionalArgument(args, 3);
                string? dataSource = GetOptionalArgument(args, 4)?.ToLowerInvariant();
                if (table is "annotation" or "activitymimeattachment" &&
                    column is not null && column.Equals("retained", StringComparison.OrdinalIgnoreCase) ||
                    table is "annotation" or "activitymimeattachment" &&
                    column is not null && column.Equals("bin", StringComparison.OrdinalIgnoreCase))
                {
                    dataSource = column.ToLowerInvariant();
                    column = null;
                }
                FileSasUrlResponse sas = DataverseFileOperations.GetFileSasUrl(
                    service, new EntityReference(table, ParseGuid(args[2])), column, dataSource);
                Console.WriteLine($"File: {sas.FileName} ({sas.FileSizeInBytes:N0} bytes, {sas.MimeType})");
                Console.WriteLine($"SAS URL (anonymous access for one hour): {sas.SasUrl}");
                break;
        }
    }

    private static Entity CreateAttachment(Guid emailId, FileInfo file) => new("activitymimeattachment")
    {
        ["objectid"] = new EntityReference("email", emailId),
        ["objecttypecode"] = "email",
        ["subject"] = $"Attached {file.Name}",
        ["filename"] = file.Name
    };

    private static Entity CreateNote(string regardingTable, Guid regardingId, FileInfo file)
    {
        Guid noteId = Guid.NewGuid();
        return new Entity("annotation", noteId)
        {
            ["annotationid"] = noteId,
            ["objectid"] = new EntityReference(regardingTable, regardingId),
            ["subject"] = $"Attached {file.Name}",
            ["notetext"] = "Created by the Dataverse SDK chunked file sample.",
            ["filename"] = file.Name
        };
    }

    private static void PrintEntity(Entity entity)
    {
        Console.WriteLine($"{entity.LogicalName}: {entity.Id}");
        foreach ((string key, object value) in entity.Attributes) Console.WriteLine($"{key}: {value}");
    }

    private static void PrintEntities(EntityCollection records)
    {
        Console.WriteLine($"Files found: {records.Entities.Count}");
        foreach (Entity record in records.Entities) PrintEntity(record);
    }

    private static void WriteDownload((byte[] Bytes, string FileName) download, string outputPath)
    {
        WriteBytes(download.Bytes, outputPath);
        Console.WriteLine($"Dataverse file name: {download.FileName}");
    }

    private static void WriteBytes(byte[] bytes, string outputPath)
    {
        string fullPath = Path.GetFullPath(outputPath);
        if (File.Exists(fullPath))
        {
            throw new IOException($"Output file already exists: {fullPath}");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllBytes(fullPath, bytes);
        Console.WriteLine($"Wrote {bytes.LongLength:N0} bytes to {fullPath}");
    }

    private static Guid ParseGuid(string value) =>
        Guid.TryParse(value, out Guid id) ? id : throw new ArgumentException($"'{value}' is not a valid GUID.");

    private static string? GetOptionalArgument(string[] args, int index) =>
        args.ElementAtOrDefault(index) is string value && value != "--confirm" && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;

    private static void RequireArguments(string[] args, int requiredCount, string usage)
    {
        if (args.Length < requiredCount) throw new ArgumentException($"Usage: {usage}");
    }

    private static void RequireConfirmed(string[] args, int requiredCount, string usage)
    {
        RequireArguments(args, requiredCount, usage);
        if (!args.Contains("--confirm", StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"This command changes data or grants anonymous file access. Usage: {usage}");
        }
    }
}