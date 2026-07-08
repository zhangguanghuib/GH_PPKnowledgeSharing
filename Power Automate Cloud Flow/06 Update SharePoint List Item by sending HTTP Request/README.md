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
<img width="2145" height="653" alt="image" src="https://github.com/user-attachments/assets/53bb2178-3adc-42d8-b00b-5d7db4144b90" /><br/>

<img width="2164" height="1257" alt="image" src="https://github.com/user-attachments/assets/0fdcb03b-52de-4490-be1f-2386bf54657f" /><br/>

## Run the flow, it shows successfully <br/>
<img width="924" height="631" alt="image" src="https://github.com/user-attachments/assets/b8112549-99e4-4060-a4ab-9132637fa9be" />


