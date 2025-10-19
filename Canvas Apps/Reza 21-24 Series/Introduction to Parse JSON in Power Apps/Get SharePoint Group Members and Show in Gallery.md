# Share Point Group Member

1. Build the below Power Automate to get SharePoint Group Members:<br/>
   <img width="1675" height="848" alt="image" src="https://github.com/user-attachments/assets/d9188194-345a-47c6-8235-b855e22c268b" /><br/>
 <img width="1513" height="871" alt="image" src="https://github.com/user-attachments/assets/51dd838e-7eeb-4ded-b263-0c231e076a88" />
 <br/>
 The Http Response is :<br/>
<img width="1198" height="1164" alt="image" src="https://github.com/user-attachments/assets/f6edca0e-2a5e-4a34-9569-c160656cb923" /><br/>
Parse Json<br/>
<img width="1615" height="875" alt="image" src="https://github.com/user-attachments/assets/72aab7a6-31e5-42d1-9c67-d43a7dc03685" /><br/>

Select <br/>
<img width="1639" height="757" alt="image" src="https://github.com/user-attachments/assets/0161af33-2d1e-4a6c-9e8a-41fb9bbff34e" /><br/>
Return to Power Apss<br/>
<img width="1601" height="735" alt="image" src="https://github.com/user-attachments/assets/c3b6c6b0-332d-4356-9daf-b8fd7b3f64b4" /><br/>
2. In Power Apps, run the flow, get the group members, and then shows in the gallery<br/>
```
Set(SharePointResponse, GetSharePointGroupMembers.Run(txtInputGroupName.Value).response); Set(call, 1)
```
<img width="1246" height="479" alt="image" src="https://github.com/user-attachments/assets/3ad1c278-a6f7-4cd4-a5c6-8b98c4c03e6d" />
<br/>
Set the gallery data source(Items of gallery) as: 
```
Table(ParseJSON(SharePointResponse))
```
Set individual gallery field value:<br/>
```
Text(ThisItem.Value.Title)
Text(ThisItem.Value.Email)
```
