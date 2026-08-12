<img width="1322" height="608" alt="image" src="https://github.com/user-attachments/assets/2ed40b94-58fb-4090-8f6b-3a02e21f7518" /># Azure AI Search connected to Copilot Studio

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
<img width="2416" height="560" alt="image" src="https://github.com/user-attachments/assets/a71ea3ed-67ae-4611-86aa-444203195703" /><br/>

## 2.  Create Azure AI Search
Inside the same resource group, create Azure AI Search<br/>
<img width="747" height="685" alt="image" src="https://github.com/user-attachments/assets/da4a5615-3f7f-46eb-8103-5de8bc386c87" /><br/>

<img width="1393" height="947" alt="image" src="https://github.com/user-attachments/assets/4fe07beb-93e5-46da-926d-a7fc4aa8f669" /><br/>

Click "Create" button<br/>

## 3. Create Azure Open AI
Create Azure Open AI Resource:<br/>
<img width="789" height="654" alt="image" src="https://github.com/user-attachments/assets/44413d71-88b2-4087-a29f-d6a9f3a4a055" /><br/>
<img width="1129" height="1093" alt="image" src="https://github.com/user-attachments/assets/4002d566-5023-4efb-a889-6ad969a54bc3" /><br/>
<img width="876" height="664" alt="image" src="https://github.com/user-attachments/assets/1284c621-cf31-4094-a1f8-c8d35427a80a" /><br/>
Wait until the deployment is done<br/>
<img width="1864" height="1205" alt="image" src="https://github.com/user-attachments/assets/2d01f032-8750-4118-892a-fdf40c34b120" /><br/>
Click go to foundry portal <br/>
Click "New Foundry"<br/>
<img width="2278" height="1341" alt="image" src="https://github.com/user-attachments/assets/8c5dccc3-9518-44d3-8c10-91952077d6f6" /><br/>

Click deploy a base model<br/>
<img width="2458" height="945" alt="image" src="https://github.com/user-attachments/assets/26a79486-c85f-4a35-a1ab-24c62fe2a3e8" /><br/>
Choose all models, and then text-embedding-3-large<br/>
<img width="2421" height="1335" alt="image" src="https://github.com/user-attachments/assets/cac03839-52cd-4374-a521-1962dae4da47" /><br/>

<img width="2476" height="1204" alt="image" src="https://github.com/user-attachments/assets/3275f8eb-69b9-4edf-aa58-85df7ee7a135" /><br/>
<img width="2470" height="1323" alt="image" src="https://github.com/user-attachments/assets/42bed868-45e6-4d8a-8d76-501e89a44670" /><br/>

See the model is deployed succesfully.

## 4 Import Document to Azure AI Search
1. Go back to azure ai search, import data:<br/>
<img width="2127" height="1234" alt="image" src="https://github.com/user-attachments/assets/dc5cd1cc-2c67-4cd6-8e95-9f44619ba864" /><br/>
<img width="2242" height="582" alt="image" src="https://github.com/user-attachments/assets/37dba257-bcfa-44d5-a1f2-051963e19b4e" /><br/>
<img width="1286" height="340" alt="image" src="https://github.com/user-attachments/assets/b9dbc193-fa37-48cd-89ad-c4b82125544e" /><br/>
<img width="1526" height="728" alt="image" src="https://github.com/user-attachments/assets/2c6757ad-525d-44ed-b5d3-d5d958c97937" /><br/>

2. Trouble shooting, you may see this error:
   <img width="1759" height="957" alt="image" src="https://github.com/user-attachments/assets/dcb358e0-8513-45b0-90c4-51a983926080" /><br/>
   Firstly, find the ai search resource->Identity<br/>
   Set “System Assigned ” = On, click save<br/>
   <img width="1387" height="702" alt="image" src="https://github.com/user-attachments/assets/b30e5c98-ca14-4b15-8531-589e9168dabb" /><br/>

   Go back to the storage account, add a role assignment, find the role:<br/>
   ```
    Storage Blob Data Reader
   ```
   <img width="2385" height="602" alt="image" src="https://github.com/user-attachments/assets/85d31444-09cb-418c-8e88-ecaf1425db77" /><br/>
   Find the ai search resource we just created<br/>
   <img width="2472" height="1130" alt="image" src="https://github.com/user-attachments/assets/9c0360a1-6e70-4faf-9b43-007e63398cce" /><br/>
   <img width="1388" height="1140" alt="image" src="https://github.com/user-attachments/assets/68637be0-14f5-4bf5-9962-c08a968c2bd6" /><br/>





