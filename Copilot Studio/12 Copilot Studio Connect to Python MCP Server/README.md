# Connect to Python MCP Server from Microsoft Copilot Studio
## This article will introduce step by step how to
1. Create MCP Server using Python language
2. Deploy MCP Server to Azure Web App
3. Test MCP server through MCP inspector
4. Connector MCP Server from Copilot Studio
5. Trigger MCP Server from Copilot Studio.

# Details
1. Create MCP Server using Python
   Step 1: Create a folder as below <br/>
   <img width="1013" height="258" alt="image" src="https://github.com/user-attachments/assets/037798e5-768f-4e7a-aedd-7fd4853e3e0b" /><br/>
   Open this folder with VS Code, create a txt file named requirements.txt inside the folder<br/>
   ```
   mcp[cli]
   ```
   <img width="1422" height="1006" alt="image" src="https://github.com/user-attachments/assets/db65e565-3087-43e3-9583-6af9c347f808" /><br/>

   Step 02:  Create virtual environment inside this folder and then activate:<br/>
   ```
   uv venv mcpdemo02
   .\mcpdemo02\Scripts\activate
   ```
  <img width="1131" height="310" alt="image" src="https://github.com/user-attachments/assets/9b130669-e3f1-49de-807f-eb4a1687a532" /><br/>

  Step 03:  Install mcp[cli] package<br/>
  ```
  uv pip install -r requirements.txt
  ```
 <img width="1606" height="1564" alt="image" src="https://github.com/user-attachments/assets/8d7579ef-ffa2-423c-bc7f-7827f6d3c578" /><br/>

 Step 04: Set the default Python Interpreter
 ```
{
    "python.defaultInterpreterPath": "C:/D/CanvasAppMCP/MCP-DEMO02/mcpdemo02/Scripts/python.exe"
}
 ```
