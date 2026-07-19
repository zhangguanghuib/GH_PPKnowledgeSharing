# Copilot Studio Connect to Python MCP Server with OAuth2.0

## Step 01  Create App Registration for MCP Server
<img width="1331" height="1452" alt="image" src="https://github.com/user-attachments/assets/c18369eb-3fda-48c0-b780-558d66cd8897" /><br/>
Click "Register" to create the App for Copilot Studio MCP Server<br/>
Expose an API->Add a scope <br/>
<img width="2144" height="1438" alt="image" src="https://github.com/user-attachments/assets/0c89d193-2466-49a6-9f29-5bc537873d06" /><br/>
Manifest:<br/>
<img width="2400" height="1291" alt="image" src="https://github.com/user-attachments/assets/da344c21-f303-4771-8726-4deaed96aac6" />

## Step 02  Create App Registration for MCP Client
<img width="1381" height="1378" alt="image" src="https://github.com/user-attachments/assets/1d7072c1-4bda-4e3c-9c5f-644938dd9d80" /><br/>
Add an API Permission, choose the App Id created in the step 01, that is MCP Server App<br/>
<img width="2471" height="1076" alt="image" src="https://github.com/user-attachments/assets/5a88072a-a1c0-4074-bdc6-de8f5cc58901" /><br/>
Choose the mcp.invoke, and then click "Add permission"<br/>
<img width="2467" height="1447" alt="image" src="https://github.com/user-attachments/assets/b3f24476-38cd-4c27-bb0b-aaf0bbb2c030" /><br/>
Create client secret<br/>
<img width="1647" height="965" alt="image" src="https://github.com/user-attachments/assets/aebc0dbe-9df8-4bfe-822b-d6884dbad992" /><br/>
Copy the secret value:<br/>
<img width="1987" height="1008" alt="image" src="https://github.com/user-attachments/assets/397d3f2c-db86-4e32-a6f1-5747b637bccc" /><br/>
API Permission, grant admin consent:<br/>
<img width="2182" height="1079" alt="image" src="https://github.com/user-attachments/assets/b9a5f974-85b8-47e7-9c96-be01b263af81" /><br/>
## Step 02  Go back to the MCP Server App, update the MCP Server App
<img width="2205" height="1292" alt="image" src="https://github.com/user-attachments/assets/a002851b-a473-4c8e-aad2-5216989e99d4" /><br/>
Add the Client Id:<br/>

<img width="2146" height="1443" alt="image" src="https://github.com/user-attachments/assets/f645876a-0d16-46ec-aec9-ebf2c7bee8ed" /><br/>

<img width="1913" height="1146" alt="image" src="https://github.com/user-attachments/assets/c43b016b-6a0f-4204-8c91-f6d50ff53a9b" /><br/>

## Step 04  Develop MCP Server using FastMCP
Create an empty folder and open it in Visual Studio Code<br/>
<img width="2006" height="1146" alt="image" src="https://github.com/user-attachments/assets/b827d5c9-daf9-49b5-8aaa-513600404752" /><br/>
Create a Python Virtual environment with uv<br/>
```
uv init
uv venv
.venv\Scripts\activate
```
<img width="1963" height="1233" alt="image" src="https://github.com/user-attachments/assets/bfd71e78-5b9e-4671-994c-a32e641dfeb8" /><br/>

Create .env file:<br/>
<img width="1588" height="638" alt="image" src="https://github.com/user-attachments/assets/98832507-8aaf-4a0f-b9c2-2b0fe89ad2fa" /><br/>

Create requirements.txt<br/>
<img width="1744" height="802" alt="image" src="https://github.com/user-attachments/assets/7f64bd01-a68f-432e-a98c-afc2f011bcf7" /><br/>
Install Python Package in the requirements.txt
```
uv pip install -r requirements.txt
```
<img width="2111" height="1573" alt="image" src="https://github.com/user-attachments/assets/725f6c4a-d8e1-47fa-968f-8be2f46bf8a1" /><br/>
Create server.py<br/>

<img width="2116" height="1174" alt="image" src="https://github.com/user-attachments/assets/9914bb55-37fa-4f93-b54b-486b49bdc69d" /><br/>

## Step 05 Create Web App in Azure Portal to deploy the MCP Server
Create Web App<br/>
<img width="987" height="1003" alt="image" src="https://github.com/user-attachments/assets/912f9ae3-2591-4d7b-834e-dd8e9e7808c6" /><br/>
<img width="1226" height="1382" alt="image" src="https://github.com/user-attachments/assets/624bdbb8-2c87-46ee-bed7-23bb1403aaac" /><br/>
Once deployment is done:<br/>
```
pip install -r requirements.txt && python server.py
```
<img width="1796" height="1194" alt="image" src="https://github.com/user-attachments/assets/ea360cc9-2204-4168-9022-c73e37192fbe" /><br/>

## Step 06 Deploy the code to Web App Service
Update .env file and deploy<br/>
<img width="2174" height="1054" alt="image" src="https://github.com/user-attachments/assets/fe8626e0-0847-4e5e-a671-0b8c3f593635" /><br/>
<img width="1734" height="1185" alt="image" src="https://github.com/user-attachments/assets/3d2b8cab-3a37-4023-85fe-25023218084e" /><br/>
<img width="1903" height="1066" alt="image" src="https://github.com/user-attachments/assets/a0c8f7c5-b0b7-40d0-b2bb-f4a09d8303e4" /><br/>

## Step 07  Connect MCP Server from COpilot Studio
<img width="1932" height="1221" alt="image" src="https://github.com/user-attachments/assets/c07e3fb2-316f-47f4-9626-64f3f4af472d" /><br/>
<img width="1529" height="1102" alt="image" src="https://github.com/user-attachments/assets/0239b69a-7b77-4924-868b-955b2e93316a" /><br/>
```
MCP Demo14
MCP Demo14 with OAuth2.0 for testing purpose
*********(Copilot Client Id)
*******(Client Secret Value)

https://login.microsoftonline.com/<tenant id>/oauth2/v2.0/authorize
https://login.microsoftonline.com/<tenant id>/oauth2/v2.0/token
https://login.microsoftonline.com/<tenant id>/oauth2/v2.0/token
api://<MCP Server App Id>/mcp.invoke
```
<img width="1050" height="947" alt="image" src="https://github.com/user-attachments/assets/07742495-9038-4203-9a25-d3c3231363a2" /><br/>

## Step 08,  update the reply URL for the Copilot MCP Client:
Click "Create" button<br/>
Then copy the generated reply URL<br/>
<img width="1535" height="1107" alt="image" src="https://github.com/user-attachments/assets/7ab587a2-29d0-491f-904f-9087a1147475" /><br/>
<img width="2362" height="1108" alt="image" src="https://github.com/user-attachments/assets/8aa4c4f5-83f6-47bf-b6ba-e2ccbf2a50dd" /><br/>
<img width="1974" height="996" alt="image" src="https://github.com/user-attachments/assets/5067ace6-44a6-4bfa-b0d2-c4685025022b" /><br/>

Create Connection:<br/>
<img width="1546" height="1126" alt="image" src="https://github.com/user-attachments/assets/a2fdf62e-7285-48ad-a201-af2a4fa947b4" /><br/>
<img width="1563" height="1132" alt="image" src="https://github.com/user-attachments/assets/5ac001a1-3d4b-4c06-9d20-a767a77e0bef" /><br/>
<img width="1534" height="1126" alt="image" src="https://github.com/user-attachments/assets/1812d0af-cbbf-4fff-ac39-722fe47ac138" /><br/>

You can see the MCP is loaded successfully<br/>
<img width="1494" height="1275" alt="image" src="https://github.com/user-attachments/assets/c728b4c7-700b-41f0-9d5a-c3a31e3f9c66" /><br/>

## Step 09, test the MCP tool in Copilot Studio<br/>
```
Please help add two numbers 9 and 8 with the tool 'MCP Demo 16'
```
<img width="2386" height="1104" alt="image" src="https://github.com/user-attachments/assets/30ad7eea-2168-4252-b4a6-470ab2249c7c" /><br/>

Check the App Service Logs, and confirm the MCP Server is called successfully<br/>
<img width="2409" height="1281" alt="image" src="https://github.com/user-attachments/assets/a1b41fe2-7512-4427-a7a5-bd770e734a6e" /><br/>



