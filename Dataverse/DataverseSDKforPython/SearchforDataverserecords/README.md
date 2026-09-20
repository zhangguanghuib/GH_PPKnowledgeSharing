# Search for Dataverse records

Executable .NET 8 examples for the highlighted Dataverse Search documentation branch. Defaults target `https://org5efadec2.crm.dynamics.com/` and use `guazha@dynamicsFTEGCR.onmicrosoft.com` as the sign-in hint. No password, token, or secret is stored.

## Run

```powershell
dotnet restore
dotnet run -- query "Contoso"
dotnet run -- suggest "Cont"
dotnet run -- autocomplete "Con"
dotnet run -- status
dotnet run -- statistics
dotnet run -- all "Contoso"
dotnet run -- enabled
dotnet run -- table-settings account
dotnet run -- legacy "Contoso"
dotnet run -- analyzer-list
```

`all` waits between calls because Dataverse Search allows one request per second per user and 150 requests per minute per organization.

## Bulk Operations

The project includes the SDK examples from [Use bulk operation messages](https://learn.microsoft.com/power-apps/developer/data-platform/bulk-operations?tabs=sdk). Check table support without changing data:

```powershell
dotnet run -- bulk-availability account
```

Run `CreateMultiple` and `UpdateMultiple` against temporary account records. The command deletes the sample records when it finishes:

```powershell
dotnet run -- bulk-create-update 3 --confirm
```

Run `UpsertMultiple` against a table with a text alternate key and a writable text description column. This follows Microsoft's `samples_bankaccount` example and cleans up records returned by the operation:

```powershell
dotnet run -- bulk-upsert samples_bankaccount samples_accountname samples_description --confirm
```

`DeleteMultiple` applies only to elastic tables. Supply the logical names, shared partition ID, and one or more record IDs:

```powershell
dotnet run -- bulk-delete-elastic contoso_sensordata contoso_sensordataid deviceid-001 3f56361a-b210-4a74-8708-3c664038fa41 --confirm
```

Standard-table bulk operations are transactional. Elastic-table operations can partially succeed. Microsoft recommends starting with 100-1,000 small records per request for standard tables and 100 per request for elastic tables.

## File and Image Data

Read file/image metadata and discover image capabilities without changing data:

```powershell
dotnet run -- file-max account sample_filecolumn
dotnet run -- file-info account <record-guid> sample_filecolumn
dotnet run -- file-related account <record-guid>
dotnet run -- image-max account sample_imagecolumn
dotnet run -- image-full-columns
dotnet run -- image-primary-columns
```

Upload, download, and delete file-column or image-column data. Upload/delete operations require confirmation:

```powershell
dotnet run -- file-upload account <record-guid> sample_filecolumn C:\Files\sample.pdf --confirm
dotnet run -- file-download account <record-guid> sample_filecolumn C:\Files\downloaded.pdf
dotnet run -- file-delete <file-guid> --confirm
dotnet run -- image-upload account <record-guid> sample_imagecolumn C:\Files\photo.png --confirm
dotnet run -- image-thumbnail account <record-guid> sample_imagecolumn C:\Files\thumbnail.png
dotnet run -- image-update account <record-guid> sample_imagecolumn C:\Files\photo.png --confirm
dotnet run -- image-delete account <record-guid> sample_imagecolumn --confirm
```

`image-upload` uses the file block messages and retrieves full-sized images when supported. `image-update` uses the direct `byte[]` record update shown in the image article. Valid image formats are GIF, JPEG, BMP, and PNG; image columns have a maximum limit of 30 MB.

## Attachment and Note Files

Check the environment-wide attachment limit, then use chunked SDK messages or direct Base64 columns. Direct uploads are intended for small files (for example, under 4 MB):

```powershell
dotnet run -- attachment-max
dotnet run -- attachment-upload <email-guid> C:\Files\sample.pdf --confirm
dotnet run -- attachment-upload-direct <email-guid> C:\Files\small.txt --confirm
dotnet run -- attachment-download <attachment-guid> C:\Files\attachment.pdf
dotnet run -- note-upload account <account-guid> C:\Files\sample.pdf --confirm
dotnet run -- note-upload-direct account <account-guid> C:\Files\small.txt --confirm
dotnet run -- note-download <note-guid> C:\Files\note.pdf
```

## Shared Access Signature URL

Generate a one-hour anonymous download URL. Treat the URL as a secret while it is valid. For notes and email attachments, omit the column name; for deleted or retained records, optionally use `bin` or `retained`.

```powershell
dotnet run -- file-sas-url account <record-guid> sample_filecolumn --confirm
dotnet run -- file-sas-url annotation <note-guid> --confirm
dotnet run -- file-sas-url annotation <deleted-note-guid> bin --confirm
```

SAS URLs work only for image columns configured to store full-sized images and aren't supported in environments still using BYOK.

## Restore Deleted Records

These commands implement the SDK examples from [Restore deleted records with code](https://learn.microsoft.com/power-apps/developer/data-platform/restore-deleted-records?tabs=sdk). Deleted record keeping must first be enabled by an administrator. Records remain restorable for at most 30 days.

List up to three deleted account records using either documented query style:

```powershell
dotnet run -- deleted-list-fetchxml 3
dotnet run -- deleted-list-query 3
```

List tables whose deleted record keeping configuration is active and ready:

```powershell
dotnet run -- deleted-enabled-tables
```

Restore by primary key. The optional name overwrites the restored account name; omit it to preserve all original values:

```powershell
dotnet run -- deleted-restore-account 00000000-0000-0000-0000-000000000000 --confirm
dotnet run -- deleted-restore-account 00000000-0000-0000-0000-000000000000 "Restored account" --confirm
```

Change a table-specific retention period (1-30 days):

```powershell
dotnet run -- deleted-set-retention account 15 --confirm
```

Restore and retention commands change production data and require `--confirm`. Restore currently supports primary keys only, not alternate keys. Restore related records first when deleted relationships would otherwise reference missing records.

## Analyzer Configuration

Analyzer settings modify production search behavior and can take 15 minutes or longer to synchronize. Listing is safe:

```powershell
dotnet run -- analyzer-list contact jobtitle
```

Writing requires explicit confirmation. Existing settings are preserved unless `--overwrite` is also provided:

```powershell
dotnet run -- analyzer-set contact jobtitle msdyn_search_remove_parenthesis_analyzer --confirm
dotnet run -- analyzer-set contact jobtitle keyword --confirm --overwrite
```

The program blocks analyzer changes to a table's primary name column, as recommended by Microsoft.

## Configuration

Override defaults with `DATAVERSE_URL`, `DATAVERSE_USER`, and `DATAVERSE_CLIENT_ID`. The built-in client ID is Microsoft's public development sample; use a tenant-owned public client application in production.

## Documentation Covered

- [Search for Dataverse records](https://learn.microsoft.com/power-apps/developer/data-platform/search/overview)
- [Dataverse Search query](https://learn.microsoft.com/power-apps/developer/data-platform/search/query)
- [Dataverse Search suggest](https://learn.microsoft.com/power-apps/developer/data-platform/search/suggest)
- [Dataverse Search autocomplete](https://learn.microsoft.com/power-apps/developer/data-platform/search/autocomplete)
- [Statistics and status](https://learn.microsoft.com/power-apps/developer/data-platform/search/statistics-status)
- [Dataverse search legacy endpoint](https://learn.microsoft.com/power-apps/developer/data-platform/search/legacy)
- [Configure Azure AI built-in analyzers](https://learn.microsoft.com/power-apps/developer/data-platform/search/custom-search-analyzer)
- [Use bulk operation messages](https://learn.microsoft.com/power-apps/developer/data-platform/bulk-operations?tabs=sdk)
- [Use file column data](https://learn.microsoft.com/power-apps/developer/data-platform/file-column-data?tabs=sdk)
- [Use image column data](https://learn.microsoft.com/power-apps/developer/data-platform/image-column-data?tabs=sdk)
- [Use file data with Attachment and Note records](https://learn.microsoft.com/power-apps/developer/data-platform/attachment-annotation-files?tabs=sdk)
- [Grant limited file access using SAS URLs](https://learn.microsoft.com/power-apps/developer/data-platform/getfilesasurl?tabs=sdk)
- [Restore deleted records with code](https://learn.microsoft.com/power-apps/developer/data-platform/restore-deleted-records?tabs=sdk)

Modern calls use the documented `/api/search/v2.0/` endpoints. SDK calls handle organization metadata and `SearchAttributeSettings`. This avoids requiring generated custom-message proxy classes at runtime.