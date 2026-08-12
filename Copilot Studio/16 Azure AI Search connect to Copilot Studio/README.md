# Azure AI Search connected to Copilot Studio

## 0.  Create Resource Group:
<img width="931" height="499" alt="image" src="https://github.com/user-attachments/assets/22ac00fb-b724-4e7c-9b08-e1b01519d805" /><br/>

## 1.  Create Storage Account
Create storage account inside the resource group:<br/>
<img width="1161" height="1030" alt="image" src="https://github.com/user-attachments/assets/e284ea65-724c-479e-bd24-029460b3ae14" /><br/>
<img width="1124" height="1361" alt="image" src="https://github.com/user-attachments/assets/1c150e48-aecd-4e26-b32f-762815833988" /><br/>
<img width="1094" height="446" alt="image" src="https://github.com/user-attachments/assets/a7efd138-3546-48ff-bebe-b386cca95548" /><br/>
Click "Review and Create"<br/>
Add a role assignment <br/>
<img width="1633" height="1141" alt="image" src="https://github.com/user-attachments/assets/df2b9cba-f005-4a16-8ca5-b5ee801f7183" /><br/>
Search
```
Storage blob Data Contributor
```
<img width="1420" height="624" alt="image" src="https://github.com/user-attachments/assets/0b7600b8-797d-4649-8a60-81e837893288" /><br/>
<img width="1392" height="728" alt="image" src="https://github.com/user-attachments/assets/0274df24-0622-4a61-858b-8885c9ca7ebc" /><br/>
Click until review + assign <br/>

Create a container<br/>
<img width="2467" height="1186" alt="image" src="https://github.com/user-attachments/assets/ba733ecf-26e4-40f9-9eec-7363dcd15da0" /><br/>

Go back to "Networking", create a virtual network<br/>
<img width="1672" height="1343" alt="image" src="https://github.com/user-attachments/assets/a9979aaf-8d03-4679-9116-274c98de3b0d" /><br/>
<img width="2458" height="1215" alt="image" src="https://github.com/user-attachments/assets/53c66f00-06b7-49f9-a8a0-4b6a92777ea7" /><br/>

Trouble shooting<br/>
If you try to open the container but see the below error<br/>
<img width="1683" height="625" alt="image" src="https://github.com/user-attachments/assets/8d05485d-ea90-4b23-91f7-89f5da872977" /><br/>

Please run the two PowerShell script<br/>
```
Invoke-RestMethod "https://api.ipify.org?format=json"
Invoke-RestMethod "https://ifconfig.me/ip"
```
<img width="1012" height="259" alt="image" src="https://github.com/user-attachments/assets/e59dbab7-de82-4f78-b3fb-e94b527eceb8" /><br/>
<img width="832" height="163" alt="image" src="https://github.com/user-attachments/assets/b787fe55-4385-4070-8796-fbf79187915d" /><br/>

Add the two IP Addresses into :<br/>
<img width="1607" height="1124" alt="image" src="https://github.com/user-attachments/assets/26c9afbb-0323-4b5b-a230-b55edca45192" /><br/>

Then you will be able open the container and upload the file<br/>
<img width="2416" height="560" alt="image" src="https://github.com/user-attachments/assets/a71ea3ed-67ae-4611-86aa-444203195703" />



