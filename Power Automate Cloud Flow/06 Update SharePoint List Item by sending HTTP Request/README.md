## Issue description
"Save Conflict.\n\nYour changes conflict with those made concurrently by another user. If you want your changes to be applied, click Back in your Web browser, refresh the page, and resubmit your changes<br/>
<img width="2701" height="1144" alt="image" src="https://github.com/user-attachments/assets/ed4c0f8c-9126-4f65-9e82-9b88de1a16ca" /><br/>

## Root cause
SharePoint uses optimistic concurrency. Every list item has an internal version stamp (ETag). When two
writers try to update the same item—or the same underlying storage row—without matching ETags,
SharePoint returns HTTP 400 “Save Conflict”. This is a client-visible symptom of a concurrency collision,
not a permanent error.<br/>

## Possible solution

<img width="1073" height="962" alt="image" src="https://github.com/user-attachments/assets/c4172f66-2df1-48b4-8952-65ecd3f26ed4" /><br/>
The flow is:<br/>
```
Accept: application/json;odata=nometadata
Content-Type: application/json;odata=nometadata
IF-MATCH:  *
X-HTTP-Method:  MERGE
{
  "IsValid": true
}
```

