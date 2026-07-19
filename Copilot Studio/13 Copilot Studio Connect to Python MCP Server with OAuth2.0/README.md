# Copilot Studio Connect to Python MCP Server with OAuth2.0

## Step 01  Create App Registration for MCp Server
<img width="1331" height="1452" alt="image" src="https://github.com/user-attachments/assets/c18369eb-3fda-48c0-b780-558d66cd8897" /><br/>
Click "Register" to create the App for Copilot Studio MCP Server<br/>
Expose an API->Add a scope <br/>
<img width="2144" height="1438" alt="image" src="https://github.com/user-attachments/assets/0c89d193-2466-49a6-9f29-5bc537873d06" /><br/>
Manifest:<br/>
<img width="1622" height="1292" alt="image" src="https://github.com/user-attachments/assets/2f859a53-c81c-48c6-8ce3-8b8964ccab3c" /><br/>

## Step 02  Create App Registration for Copilot Client
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

## Step 03  Develop MCP Server using FastMCP
Create an empty folder and open it in Visual Studio Code<br/>
<img width="2006" height="1146" alt="image" src="https://github.com/user-attachments/assets/b827d5c9-daf9-49b5-8aaa-513600404752" /><br/>
Create a Python Virtual environment with uv<br/>
```
uv init
uv venv
.venv\Scripts\activate
```
<img width="1963" height="1233" alt="image" src="https://github.com/user-attachments/assets/bfd71e78-5b9e-4671-994c-a32e641dfeb8" />
