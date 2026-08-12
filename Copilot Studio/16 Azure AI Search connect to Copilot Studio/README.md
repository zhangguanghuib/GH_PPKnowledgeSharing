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
   Go back to Azure AI Search, now it worked:
   <img width="1697" height="1412" alt="image" src="https://github.com/user-attachments/assets/e2a58629-57dd-4207-8ef9-6f1945112a11" /><br/>
   Find the Foundry Project and the deployed model<br/>
   <img width="1815" height="1398" alt="image" src="https://github.com/user-attachments/assets/a45b9260-352c-40ab-9bc3-eb1c8f3fa0e4" /><br/>
   <img width="1874" height="1435" alt="image" src="https://github.com/user-attachments/assets/a8b2a2ac-3dd4-4e04-83ea-f97b3ec711bd" /><br/>

3. Verify search with this question:<br/>
```
what is the maternity leave policy for new dads?
```
But got this error:
<img width="1564" height="1043" alt="image" src="https://github.com/user-attachments/assets/16b98658-765d-4b77-b668-12d0655cd4c0" /><br/>

4. Trouble shooting:
 Go to resource group, find the foundry project resource<br/>
 <img width="1791" height="959" alt="image" src="https://github.com/user-attachments/assets/b4e40e9f-a5dc-484d-8424-e6b213f3b8cd" /><br/>
 Open the Role Assignment:<br/>
 Search this role<br/>
 ```
 Cognitive Services OpenAI User
 ```
<img width="2483" height="1022" alt="image" src="https://github.com/user-attachments/assets/4e1b07df-09ce-4a7c-b2f4-353a7cbf2499" /><br/>

Go back to the Azure AI Search Resource, find the Indexer:<br/>
<img width="2018" height="1246" alt="image" src="https://github.com/user-attachments/assets/c79dc731-f38b-4af4-a499-3f0de8994e96" /><br/>

5. Reset and rerun the index<br/>
But got the different error:<br/>
<img width="2440" height="873" alt="image" src="https://github.com/user-attachments/assets/afb6d29d-8119-494e-b8f3-893c224c8ff5" /><br/>

Go to Resource group, find the Foundry resource, not "Foundry Project":<br/>
<img width="1792" height="850" alt="image" src="https://github.com/user-attachments/assets/f129f10e-6f92-4aac-af6e-59f550d21edd" /><br/>
And a role assignment, find this role<br/>
```
Cognitive Services OpenAI User
```
<img width="2491" height="913" alt="image" src="https://github.com/user-attachments/assets/b278b574-f461-4dc5-8d46-9a08ed85b308" /><br/>
Find the Azure AI Search Resource, complete assignment<br/>
<img width="2460" height="1441" alt="image" src="https://github.com/user-attachments/assets/f2e2d741-1920-49b3-aaf8-362010a3b6d7" /><br/>
Reset and Rerun the Index, you can see it can be successful<br/>
<img width="2417" height="744" alt="image" src="https://github.com/user-attachments/assets/0d9c7e52-f48a-46b7-b634-953c55eee670" /><br/>

6. Go to search explorer again:
   <img width="2222" height="1249" alt="image" src="https://github.com/user-attachments/assets/d0a73b12-a6e2-4bf6-920f-0514c47ea3ff" /><br/>
   Ask the same question, but got the same error:<br/>
   <img width="1682" height="1036" alt="image" src="https://github.com/user-attachments/assets/b93ebf7c-de48-4290-8c9d-6e2d3a808207" /><br/>
   Trouble shooting<br/>
   As per document, we should wait around 20 -30 mins, then the issue auto-resolved<br/>
   <img width="1390" height="1412" alt="image" src="https://github.com/user-attachments/assets/78238a9e-63d6-448e-83ff-43172856ebdb" /><br/>

## 5 Connect Azure AI Search to Copilot Studio
1. Get the Azure AI Search url from:
   <img width="2215" height="909" alt="image" src="https://github.com/user-attachments/assets/d1005d22-e635-4f05-8042-34f5e8951464" /><br/>

2. Get the API Key from:
   <img width="1342" height="1173" alt="image" src="https://github.com/user-attachments/assets/37c30a7d-4e70-4bbb-9a0b-e3498b9d917d" /><br/>

3. Create a Copilot Studio Agent, add "Knowledge Source", then click "Azure AI Search"
   <img width="1952" height="1286" alt="image" src="https://github.com/user-attachments/assets/05b8cc37-5b59-4071-aa9e-93039cd2950d" /><br/>
   <img width="1979" height="1322" alt="image" src="https://github.com/user-attachments/assets/af1470f2-812f-4c21-94ff-6bee7e0e2acd" /><br/>
   <img width="1936" height="1278" alt="image" src="https://github.com/user-attachments/assets/7821b903-b0e8-4255-afb0-7d8c0af12968" /><br/>
   <img width="2147" height="1234" alt="image" src="https://github.com/user-attachments/assets/0a1c6812-ec6e-46fb-a9ee-dba7cfc2cfe9" /><br/>

4. Ask the Copilot Studio Agent with the same question:
   ```
   what is the maternity leave policy for new dads?
   ```
   See it worked<br/>
   <img width="2320" height="1321" alt="image" src="https://github.com/user-attachments/assets/fe36bb4c-b0fb-49ab-8c70-cdea88869f84" /><br/>




