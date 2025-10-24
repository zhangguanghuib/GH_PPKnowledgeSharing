# Create a Modern ComboBox People Picker Experience in Power Apps
Step 1: Create a SharePoint List as below:<br/>
<img width="2005" height="754" alt="image" src="https://github.com/user-attachments/assets/f35b71ba-6186-4f6b-a070-677a5a1a7099" /><br/>

Step 2:
Add a vertical gallery <br/>
<img width="2457" height="1048" alt="image" src="https://github.com/user-attachments/assets/b84a19f1-4940-4cd4-bbc5-38999951a4a9" /><br/>

Step 3:<br/>
Adding a modern form control <br/>
<img width="368" height="593" alt="image" src="https://github.com/user-attachments/assets/10fc54a2-eecc-4b96-9118-db059ea30a12" /><br/>

Step 4: <br/>
Unlock the data card for changing it: <br/>
<img width="1228" height="656" alt="image" src="https://github.com/user-attachments/assets/27a1b796-9850-4c83-a2c9-3c2b03fe22ae" />

For the ComboBox, set its Items<br/>
```
Office365Users.SearchUserV2({searchTerm:Self.SearchText, isSearchTermRequired:false, top:999}).value
```
<img width="1816" height="991" alt="image" src="https://github.com/user-attachments/assets/6747a71e-de2f-4451-bd0d-52a254206979" /><br/>

Set its DefaultSelectedItems: <br/>
```
Office365Users.SearchUserV2({searchTerm: ThisItem.'Assigned to'.Email, top:1}).value
```
<img width="1794" height="983" alt="image" src="https://github.com/user-attachments/assets/b2188889-0df9-424a-bd6e-5ec8f9499310" />
Set its InputTextPlaceholder <br/>
```
🔍Search for user
```
<img width="2062" height="1095" alt="image" src="https://github.com/user-attachments/assets/c801e64d-6e64-4db6-b6a6-b16d999eb71d" />
