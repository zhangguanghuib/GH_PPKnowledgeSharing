# GH_PPKnowledgeSharing

The below steps comes from these document:<br/>
https://learn.microsoft.com/en-us/microsoft-copilot-studio/configure-sso?tabs=webApp<br/>
https://learn.microsoft.com/en-us/microsoft-copilot-studio/configuration-authentication-azure-ad?tabs=fic-auth<br/>
https://github.com/microsoft/CopilotStudioSamples/tree/main/sso/entra-id<br/>
<hr/>
<h1>Setp 01: Create Copilot Agent for testing:</h1>
<img width="915" height="629" alt="image" src="https://github.com/user-attachments/assets/b1e01da9-c35b-49a0-9e0a-94db36614b67" /><br/>
<img width="1665" height="1356" alt="image" src="https://github.com/user-attachments/assets/177b51ab-96b5-4a61-9cec-60a2faecbfd9" /><br/>
<hr/>
<h1>Setp 02: Create App Registration for Authentication:</h1>
<img width="1509" height="958" alt="image" src="https://github.com/user-attachments/assets/289162d7-badb-42b3-a5aa-0d42738b6957" /><br/>
Click "Register"<br/>
<img width="2458" height="1057" alt="image" src="https://github.com/user-attachments/assets/58782f86-1410-43ed-88fc-1b7771df5b7c" /><br/>
<img width="2462" height="1365" alt="image" src="https://github.com/user-attachments/assets/b59b31fc-cc9c-45cd-b313-f1d41080b725" /><br/>

Input the client id and then click "Save"<br/>
<img width="1954" height="1432" alt="image" src="https://github.com/user-attachments/assets/eac841ad-a599-4970-a7c7-a732a3bb64c1" /><br/>
<img width="1609" height="1201" alt="image" src="https://github.com/user-attachments/assets/0f2e5837-3e27-464c-96f8-40d5a264526d" /><br/>

Adding Federated credentials<br/>
<img width="2065" height="960" alt="image" src="https://github.com/user-attachments/assets/0a6c1962-b77e-4de5-bb88-65a34bde954d" /><br/>
<img width="1416" height="1175" alt="image" src="https://github.com/user-attachments/assets/399922d2-0f54-4fe3-8cd8-129fd07f13e4" /><br/>
<img width="2078" height="932" alt="image" src="https://github.com/user-attachments/assets/59eab919-e19a-472d-9137-dfab4d41a196" />
<hr/>

Add a permission: <br/>
<img width="2116" height="1429" alt="image" src="https://github.com/user-attachments/assets/560c9dd3-e70a-492b-9fcc-17f4ee0e6f84" /><br/>
Click "Add permission"<br/>
<img width="1480" height="754" alt="image" src="https://github.com/user-attachments/assets/4142f459-1767-43ce-baf6-aef1cf67ddf3" /><br/>

Adding a scope<br/>
<img width="2384" height="1096" alt="image" src="https://github.com/user-attachments/assets/db70e459-9242-42da-94b9-c0f5542ebb9b" /><br/>

<img width="1955" height="1091" alt="image" src="https://github.com/user-attachments/assets/5974b023-a64f-4144-abd5-db0bc20cd626" /><br/>
<img width="1634" height="1245" alt="image" src="https://github.com/user-attachments/assets/54232413-ff93-4fb0-99b1-4a06b58d7fd2" /><br/>
<hr/>
<h1>Step 03: Create App Registration for your website<br/></h1>
<img width="1576" height="891" alt="image" src="https://github.com/user-attachments/assets/25482d10-1dbd-4aad-9791-531ce84d676b" /><br/>

Adding a scope<br/>
<img width="2395" height="1147" alt="image" src="https://github.com/user-attachments/assets/6940e583-8fcc-44a8-bbe9-82936ad308cf" /><br/>

Authorized client applications<br/>
<img width="2378" height="1135" alt="image" src="https://github.com/user-attachments/assets/193ea7b1-c653-41e8-81b7-52092a13651e" /><br/>
<br/>
<img width="2405" height="1086" alt="image" src="https://github.com/user-attachments/assets/2e775de1-c88c-4772-89f6-6352957f56e0" /><br/>
<img width="2493" height="1416" alt="image" src="https://github.com/user-attachments/assets/61f62566-a4ac-4228-9176-f9e05cbe5538" /><br/>

Adding Redirect URI:<br/>
<img width="1128" height="246" alt="image" src="https://github.com/user-attachments/assets/4794b6c0-7e71-4240-abcc-d6dd15bb0655" /><br/>

<img width="2459" height="1097" alt="image" src="https://github.com/user-attachments/assets/db5d2645-c975-42e5-bf35-5fa13ab26313" /><br/>

<img width="2387" height="1146" alt="image" src="https://github.com/user-attachments/assets/79e3da1a-c3df-4eef-9f4d-4343aaae3757" /><br/>

Get Token Endpoint<br/>
<img width="2366" height="1187" alt="image" src="https://github.com/user-attachments/assets/deafaebf-96fb-4a6d-ab51-11eeb8504a75" /><br/>

<hr/>
<h1>Step 04. Update the code in Github <br/><h1>
https://github.com/microsoft/CopilotStudioSamples/tree/main/sso/entra-id<br/>
<img width="1441" height="960" alt="image" src="https://github.com/user-attachments/assets/28562752-8cf5-49b6-8ff0-b31fb3bb6255" /><br/>

<hr/>  
<h1>Setp 05: How to test it?<br/></h1>h1>

Step 01, make sure you publish the Copilot Agent, otherwise you will see error.<br/>
<img width="2421" height="1242" alt="image" src="https://github.com/user-attachments/assets/c83f792c-e71c-4712-89c4-cd97cfb93b42" /><br/>

Step 02:<br/>
Create a new empty Github Repository, and only put the index.html in it:<br/>
<img width="2186" height="993" alt="image" src="https://github.com/user-attachments/assets/649f5a94-6610-4995-bca6-8170382efa22" /><br/>
Click Setting button<br/>
<img width="1764" height="901" alt="image" src="https://github.com/user-attachments/assets/06c3ddf1-5b8e-45fe-877b-060b3d40fa49" /><br/>
Click Pages->Visit Site<br/>
<img width="2039" height="1223" alt="image" src="https://github.com/user-attachments/assets/cbdc4159-13c3-4d64-9dc5-66a37ef7cdf3" /><br/>
Open the webpage: <br/>
<img width="1317" height="1529" alt="image" src="https://github.com/user-attachments/assets/2efe073c-9d8d-4bc3-8c6f-32dce3c4390a" /><br/>
Copy the code<br/>
<img width="930" height="352" alt="image" src="https://github.com/user-attachments/assets/e5f83442-73da-42b9-ba36-72dcdef47c98" /><br/>
<img width="1103" height="1540" alt="image" src="https://github.com/user-attachments/assets/1160e29c-cd31-43e6-a6e3-05167bf0c477" /><br/>

You can see the bot is working fine<br/>
<img width="2495" height="1346" alt="image" src="https://github.com/user-attachments/assets/ae236486-3af6-4ba5-adbf-ac81a3db3e09" />
