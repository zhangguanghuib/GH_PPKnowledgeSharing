# Azure Monitor Logs Connector Configuration In Copilot Studio

## 1. Refer official document of Azure Monitor Logs Connector 
```
https://learn.microsoft.com/en-us/connectors/azuremonitorlogs/
```

## 2. Log on to https://portal.azure.com,  create a resource group needed

<img width="1244" height="538" alt="image" src="https://github.com/user-attachments/assets/9eaff766-cad2-4044-8736-6ee856872a55" /><br/>
Open the Resource Groups<br/>
<img width="2460" height="772" alt="image" src="https://github.com/user-attachments/assets/ada5571b-86d5-42a1-9df0-a69568225e7a" /><br/>
Click "Create" <br/>

3. Create "Log Analytics workspaces"<br/>
   <img width="1324" height="887" alt="image" src="https://github.com/user-attachments/assets/4b5ece05-edea-414c-b313-2b1e3670c4fd" /><br/>
   Give it a name:<br/>
   <img width="1260" height="1388" alt="image" src="https://github.com/user-attachments/assets/fa929b64-7c62-4e3d-b152-629d2d54259e" /><br/>
   Once the deployment is done, you can open the workspace<br/>
   <img width="2450" height="797" alt="image" src="https://github.com/user-attachments/assets/da8715b9-fe46-418c-bec9-68ffce502732" /><br/>

4. Create "Microsoft Sentinel "

   <img width="1858" height="428" alt="image" src="https://github.com/user-attachments/assets/bc74b160-2c37-4d12-b87f-d6f60e6562fd" /><br/>
   Click "Create" <br/>
   <img width="1509" height="302" alt="image" src="https://github.com/user-attachments/assets/7b72d7f1-4458-46d3-848a-804d602bba91" /><br/>
   Select the workspace created before, and then click Add button<br/>
   <img width="1422" height="1378" alt="image" src="https://github.com/user-attachments/assets/fbebf06d-fe47-42b7-82e2-c173ccca5e04" /><br/>

5. Configure the Data Connectors:<br/>
   <img width="1909" height="1383" alt="image" src="https://github.com/user-attachments/assets/fbd82b88-a181-4a1e-bb63-4b3aced3ded9" /><br/>
   You can see currently there is only 7 connectors.
   Click "Content Hub":<br/>
   <img width="1856" height="1279" alt="image" src="https://github.com/user-attachments/assets/ee5a0537-4994-470d-a935-029ac05c941d" /><br/>
   Search "Microsoft 365" <br/>
   <img width="2491" height="1464" alt="image" src="https://github.com/user-attachments/assets/2338d523-e737-4521-b9c9-ab4b51dc90d0" /><br/>
   Select this connector, and then click "Install" <br/>
   Once Installation is done, and then click "Manage" button <br/>
   <img width="2469" height="1392" alt="image" src="https://github.com/user-attachments/assets/537a161a-ce07-4d55-a073-61d64065a82d" /><br/>


