
### 1. Build a SharePoint List as below

<img width="1930" height="482" alt="image" src="https://github.com/user-attachments/assets/1c17985c-3eff-4bee-98af-bcf38ef1cee4" />

| Column Name | Column Type |
| :--- | :--- |
| Visit Date | Date and Time |
| Office Location | Location |
| Visitor | Person or Group |
| Status | Choice |

### 2. Create a new Canvas App and enable the Co-Authoring
<img width="2076" height="1274" alt="image" src="https://github.com/user-attachments/assets/8df6bd2b-4532-427c-bf29-15008eb6cc91" /><br/>

Create connections and add to this App <br/>
<img width="2441" height="849" alt="image" src="https://github.com/user-attachments/assets/e834ee90-dd6b-4edb-a46c-40a649037f9c" /><br/>

### 3. Check and install these plugin and skills
```
https://github.com/microsoft/power-platform-skills/tree/main
```
<img width="2490" height="1047" alt="image" src="https://github.com/user-attachments/assets/768365d0-59a4-452a-8349-f30830aa3097" /><br/>

### 4.  Use GitHub Copilot to build Canvas App
```text
canvas app mcp
https://make.powerapps.com/e/5e61b5c4-120e-e1fb-9fee-3073d48f0e86/canvas?action=edit&form-factor=tablet&app-id=%2Fproviders%2FMicrosoft.PowerApps%2Fapps%2Fd3d539b8-621a-4aa7-8956-6aa556a4bb67
```
Then
```text
Build the app based on the 'Office Visits' SharePoint list.
 One screen: a gallery showing my upcoming visits sorted by date,
 and a form to book a new visit. Modern controls.
```
<img width="1613" height="599" alt="image" src="https://github.com/user-attachments/assets/637a9179-fe38-4f9f-8288-f106e3201478" /><br/>

#### 5.  The App content is lost, using the below prompt to push the code again
<img width="1562" height="144" alt="image" src="https://github.com/user-attachments/assets/22d18015-a3ed-40c8-a589-4f6c67194aac" /><br/>

#### 6. Finally we got this app built <br/>
<img width="2463" height="1452" alt="image" src="https://github.com/user-attachments/assets/8c69d79b-a066-4735-bf59-951e2eab5e52" /><br/>

Check the code of "Book Visit", it looks like this: <br/>
<img width="2441" height="1291" alt="image" src="https://github.com/user-attachments/assets/0918dcf6-e5c0-4e9b-a09f-b4ba2026f7e4" /><br/>

### 7, Set the Status with default value pending <br/>
```
default the status to Pending for all new bookings
```
<br/><img width="1619" height="1101" alt="image" src="https://github.com/user-attachments/assets/194c362a-d746-4496-884f-55400e0cf36e" /><br/>

### 8. Support the Location Column
```
I have update the SharePoint List, make the Location column as a Choice Column, can you please update the Canvas App based on the new schema of the SharePoint List?
```
Now the App Works:<br/>
![Uploading image.png…]()



