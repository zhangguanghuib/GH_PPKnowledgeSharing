using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

internal static class DataverseFileOperations
{
    private const int BlockSize = 4 * 1024 * 1024;

    public static int GetFileColumnMaxSizeInKb(
        IOrganizationService service,
        string tableLogicalName,
        string columnLogicalName)
    {
        AttributeMetadata metadata = GetAttributeMetadata(service, tableLogicalName, columnLogicalName);
        return metadata is FileAttributeMetadata fileColumn
            ? fileColumn.MaxSizeInKB ?? 0
            : throw new ArgumentException($"{tableLogicalName}.{columnLogicalName} is not a file column.");
    }

    public static int GetImageColumnMaxSizeInKb(
        IOrganizationService service,
        string tableLogicalName,
        string columnLogicalName)
    {
        AttributeMetadata metadata = GetAttributeMetadata(service, tableLogicalName, columnLogicalName);
        return metadata is ImageAttributeMetadata imageColumn
            ? imageColumn.MaxSizeInKB ?? 0
            : throw new ArgumentException($"{tableLogicalName}.{columnLogicalName} is not an image column.");
    }

    public static Guid UploadFile(
        IOrganizationService service,
        EntityReference target,
        string columnLogicalName,
        FileInfo file,
        string? mimeType = null)
    {
        EnsureFileExists(file);
        InitializeFileBlocksUploadResponse initializeResponse =
            (InitializeFileBlocksUploadResponse)service.Execute(new InitializeFileBlocksUploadRequest
            {
                Target = target,
                FileAttributeName = columnLogicalName,
                FileName = file.Name
            });

        string[] blockIds = UploadBlocks(service, file, initializeResponse.FileContinuationToken);
        CommitFileBlocksUploadResponse commitResponse =
            (CommitFileBlocksUploadResponse)service.Execute(new CommitFileBlocksUploadRequest
            {
                BlockList = blockIds,
                FileContinuationToken = initializeResponse.FileContinuationToken,
                FileName = file.Name,
                MimeType = mimeType ?? GetMimeType(file.Extension)
            });
        return commitResponse.FileId;
    }

    public static (byte[] Bytes, string FileName) DownloadFile(
        IOrganizationService service,
        EntityReference target,
        string columnLogicalName)
    {
        InitializeFileBlocksDownloadResponse response =
            (InitializeFileBlocksDownloadResponse)service.Execute(new InitializeFileBlocksDownloadRequest
            {
                Target = target,
                FileAttributeName = columnLogicalName
            });
        long blockSize = response.IsChunkingSupported ? BlockSize : response.FileSizeInBytes;
        return (
            DownloadBlocks(service, response.FileContinuationToken, response.FileSizeInBytes, blockSize),
            response.FileName);
    }

    public static void DeleteFile(IOrganizationService service, Guid fileId) =>
        service.Execute(new DeleteFileRequest { FileId = fileId });

    public static Entity RetrieveRecordWithFileColumns(
        IOrganizationService service,
        string tableLogicalName,
        Guid recordId,
        params string[] columns) =>
        service.Retrieve(tableLogicalName, recordId, new ColumnSet(columns));

    public static EntityCollection GetRelatedFileInformation(
        IOrganizationService service,
        string tableLogicalName,
        Guid recordId)
    {
        Relationship relationship = new($"{tableLogicalName}_FileAttachments");
        RetrieveResponse response = (RetrieveResponse)service.Execute(new RetrieveRequest
        {
            Target = new EntityReference(tableLogicalName, recordId),
            ColumnSet = new ColumnSet(false),
            RelatedEntitiesQuery = new RelationshipQueryCollection
            {
                [relationship] = new QueryExpression("fileattachment")
                {
                    ColumnSet = new ColumnSet(
                        "createdon", "mimetype", "filesizeinbytes", "filename",
                        "regardingfieldname", "fileattachmentid")
                }
            }
        });
        return response.Entity.RelatedEntities.TryGetValue(relationship, out EntityCollection? files)
            ? files
            : new EntityCollection();
    }

    public static void PrintFullSizedImageColumns(IOrganizationService service)
    {
        QueryExpression query = new("attributeimageconfig")
        {
            ColumnSet = new ColumnSet("parententitylogicalname", "attributelogicalname")
        };
        query.Criteria.AddCondition("canstorefullimage", ConditionOperator.Equal, true);
        PrintColumnPairs(service.RetrieveMultiple(query), "parententitylogicalname", "attributelogicalname");
    }

    public static void PrintPrimaryImageColumns(IOrganizationService service)
    {
        QueryExpression query = new("entityimageconfig")
        {
            ColumnSet = new ColumnSet("parententitylogicalname", "primaryimageattribute")
        };
        PrintColumnPairs(service.RetrieveMultiple(query), "parententitylogicalname", "primaryimageattribute");
    }

    public static byte[] RetrieveThumbnail(
        IOrganizationService service,
        string tableLogicalName,
        Guid recordId,
        string imageColumnLogicalName) =>
        service.Retrieve(tableLogicalName, recordId, new ColumnSet(imageColumnLogicalName))
            .GetAttributeValue<byte[]>(imageColumnLogicalName)
        ?? throw new InvalidOperationException("The image column has no thumbnail data.");

    public static void UpdateImage(
        IOrganizationService service,
        string tableLogicalName,
        Guid recordId,
        string imageColumnLogicalName,
        byte[] imageBytes) =>
        service.Update(new Entity(tableLogicalName, recordId) { [imageColumnLogicalName] = imageBytes });

    public static void DeleteImage(
        IOrganizationService service,
        string tableLogicalName,
        Guid recordId,
        string imageColumnLogicalName) =>
        service.Update(new Entity(tableLogicalName, recordId) { [imageColumnLogicalName] = null });

    public static CommitAttachmentBlocksUploadResponse UploadAttachment(
        IOrganizationService service,
        Entity attachment,
        FileInfo file,
        string? mimeType = null)
    {
        if (attachment.LogicalName != "activitymimeattachment")
        {
            throw new ArgumentException("Target must be an activitymimeattachment.", nameof(attachment));
        }
        EnsureFileExists(file);
        attachment.Attributes.Remove("body");
        attachment["filename"] = file.Name;
        if (!attachment.Contains("mimetype")) attachment["mimetype"] = mimeType ?? GetMimeType(file.Extension);

        InitializeAttachmentBlocksUploadResponse initializeResponse =
            (InitializeAttachmentBlocksUploadResponse)service.Execute(new InitializeAttachmentBlocksUploadRequest
            {
                Target = attachment
            });
        string[] blockIds = UploadBlocks(service, file, initializeResponse.FileContinuationToken);
        return (CommitAttachmentBlocksUploadResponse)service.Execute(new CommitAttachmentBlocksUploadRequest
        {
            Target = attachment,
            BlockList = blockIds,
            FileContinuationToken = initializeResponse.FileContinuationToken
        });
    }

    public static Guid CreateAttachmentDirect(
        IOrganizationService service,
        Guid emailId,
        FileInfo file,
        string? mimeType = null)
    {
        EnsureFileExists(file);
        return service.Create(new Entity("activitymimeattachment")
        {
            ["objectid"] = new EntityReference("email", emailId),
            ["objecttypecode"] = "email",
            ["subject"] = $"Attached {file.Name}",
            ["filename"] = file.Name,
            ["mimetype"] = mimeType ?? GetMimeType(file.Extension),
            ["body"] = Convert.ToBase64String(File.ReadAllBytes(file.FullName))
        });
    }

    public static (byte[] Bytes, string FileName) DownloadAttachment(
        IOrganizationService service,
        EntityReference target)
    {
        EnsureLogicalName(target, "activitymimeattachment");
        InitializeAttachmentBlocksDownloadResponse response =
            (InitializeAttachmentBlocksDownloadResponse)service.Execute(new InitializeAttachmentBlocksDownloadRequest
            {
                Target = target
            });
        return (DownloadBlocks(service, response.FileContinuationToken, response.FileSizeInBytes, BlockSize), response.FileName);
    }

    public static CommitAnnotationBlocksUploadResponse UploadNote(
        IOrganizationService service,
        Entity annotation,
        FileInfo file,
        string? mimeType = null)
    {
        if (annotation.LogicalName != "annotation" || annotation.Id == Guid.Empty)
        {
            throw new ArgumentException("Target must be an annotation with a non-empty annotation ID.", nameof(annotation));
        }
        EnsureFileExists(file);
        annotation.Attributes.Remove("documentbody");
        annotation["filename"] = file.Name;
        if (!annotation.Contains("mimetype")) annotation["mimetype"] = mimeType ?? GetMimeType(file.Extension);

        InitializeAnnotationBlocksUploadResponse initializeResponse =
            (InitializeAnnotationBlocksUploadResponse)service.Execute(new InitializeAnnotationBlocksUploadRequest
            {
                Target = annotation
            });
        string[] blockIds = UploadBlocks(service, file, initializeResponse.FileContinuationToken);
        return (CommitAnnotationBlocksUploadResponse)service.Execute(new CommitAnnotationBlocksUploadRequest
        {
            Target = annotation,
            BlockList = blockIds,
            FileContinuationToken = initializeResponse.FileContinuationToken
        });
    }

    public static Guid CreateNoteDirect(
        IOrganizationService service,
        EntityReference regarding,
        FileInfo file,
        string? mimeType = null)
    {
        EnsureFileExists(file);
        return service.Create(new Entity("annotation")
        {
            ["objectid"] = regarding,
            ["subject"] = $"Attached {file.Name}",
            ["notetext"] = "Created by the Dataverse SDK file sample.",
            ["filename"] = file.Name,
            ["mimetype"] = mimeType ?? GetMimeType(file.Extension),
            ["documentbody"] = Convert.ToBase64String(File.ReadAllBytes(file.FullName))
        });
    }

    public static (byte[] Bytes, string FileName) DownloadNote(
        IOrganizationService service,
        EntityReference target)
    {
        EnsureLogicalName(target, "annotation");
        InitializeAnnotationBlocksDownloadResponse response =
            (InitializeAnnotationBlocksDownloadResponse)service.Execute(new InitializeAnnotationBlocksDownloadRequest
            {
                Target = target
            });
        return (DownloadBlocks(service, response.FileContinuationToken, response.FileSizeInBytes, BlockSize), response.FileName);
    }

    public static int GetMaxUploadFileSize(IOrganizationService service)
    {
        QueryExpression query = new("organization")
        {
            ColumnSet = new ColumnSet("maxuploadfilesize"),
            TopCount = 1
        };
        return service.RetrieveMultiple(query).Entities.Single().GetAttributeValue<int>("maxuploadfilesize");
    }

    public static FileSasUrlResponse GetFileSasUrl(
        IOrganizationService service,
        EntityReference target,
        string? fileAttributeName = null,
        string? dataSource = null)
    {
        GetFileSasUrlRequest request = new() { Target = target };
        if (target.LogicalName is "annotation" or "activitymimeattachment")
        {
            request.FileAttributeName = string.Empty;
        }
        else if (!string.IsNullOrWhiteSpace(fileAttributeName))
        {
            request.FileAttributeName = fileAttributeName;
        }
        else
        {
            throw new ArgumentException("A file or image column is required for this table.", nameof(fileAttributeName));
        }

        if (dataSource is not null && dataSource is not ("retained" or "bin"))
        {
            throw new ArgumentException("Data source must be 'retained' or 'bin'.", nameof(dataSource));
        }
        if (dataSource is not null) request.DataSource = dataSource;
        return ((GetFileSasUrlResponse)service.Execute(request)).Result;
    }

    private static AttributeMetadata GetAttributeMetadata(
        IOrganizationService service,
        string tableLogicalName,
        string columnLogicalName) =>
        ((RetrieveAttributeResponse)service.Execute(new RetrieveAttributeRequest
        {
            EntityLogicalName = tableLogicalName,
            LogicalName = columnLogicalName
        })).AttributeMetadata;

    private static string[] UploadBlocks(IOrganizationService service, FileInfo file, string token)
    {
        List<string> blockIds = [];
        using Stream stream = file.OpenRead();
        byte[] buffer = new byte[BlockSize];
        int bytesRead;
        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            byte[] block = bytesRead == buffer.Length ? buffer : buffer[..bytesRead];
            string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
            blockIds.Add(blockId);
            service.Execute(new UploadBlockRequest
            {
                BlockData = block,
                BlockId = blockId,
                FileContinuationToken = token
            });
        }
        return blockIds.ToArray();
    }

    private static byte[] DownloadBlocks(
        IOrganizationService service,
        string token,
        long totalBytes,
        long preferredBlockSize)
    {
        using MemoryStream output = totalBytes <= int.MaxValue ? new MemoryStream((int)totalBytes) : new MemoryStream();
        long offset = 0;
        while (offset < totalBytes)
        {
            long blockLength = Math.Min(preferredBlockSize, totalBytes - offset);
            DownloadBlockResponse response = (DownloadBlockResponse)service.Execute(new DownloadBlockRequest
            {
                BlockLength = blockLength,
                FileContinuationToken = token,
                Offset = offset
            });
            output.Write(response.Data);
            offset += response.Data.LongLength;
        }
        return output.ToArray();
    }

    private static void PrintColumnPairs(EntityCollection records, string tableColumn, string imageColumn)
    {
        Console.WriteLine($"Image columns found: {records.Entities.Count}");
        foreach (Entity record in records.Entities)
        {
            Console.WriteLine($"{record.GetAttributeValue<string>(tableColumn)}.{record.GetAttributeValue<string>(imageColumn)}");
        }
    }

    private static void EnsureFileExists(FileInfo file)
    {
        if (!file.Exists) throw new FileNotFoundException("Input file was not found.", file.FullName);
        if (file.Length == 0) throw new ArgumentException("Input file must not be empty.", nameof(file));
    }

    private static void EnsureLogicalName(EntityReference target, string expected)
    {
        if (target.LogicalName != expected) throw new ArgumentException($"Target must refer to {expected}.", nameof(target));
    }

    private static string GetMimeType(string extension) => extension.ToLowerInvariant() switch
    {
        ".bmp" => "image/bmp",
        ".gif" => "image/gif",
        ".jpeg" or ".jpg" => "image/jpeg",
        ".json" => "application/json",
        ".pdf" => "application/pdf",
        ".png" => "image/png",
        ".txt" => "text/plain",
        _ => "application/octet-stream"
    };
}