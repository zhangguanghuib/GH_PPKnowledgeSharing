# Connect to Python MCP Server from Microsoft Copilot Studio
## This article will introduce step by step how to
1. Create MCP Server using Python language
2. Deploy MCP Server to Azure Web App
3. Test MCP server through MCP inspector
4. Connector MCP Server from Copilot Studio
5. Trigger MCP Server from Copilot Studio.

# Details
1. Prepare MCP development environment
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
<img width="2029" height="999" alt="image" src="https://github.com/user-attachments/assets/25646dc2-b694-4eee-a955-876271c758da" /><br/>

2. Write the below python code

```py
from mcp.server.fastmcp import FastMCP

mcp = FastMCP("demo-mcp", stateless_http=True, auth = None, host="0.0.0.0")

@mcp.tool("add", description="Add two numbers")
def add(a: int, b: int) -> int:
    return a + b

@mcp.tool("minus", description="Subtract two numbers")
def minus(a: int, b: int) -> int:
    return a - b

if __name__ == "__main__":
    mcp.run(transport="streamable-http")
```
<img width="1767" height="819" alt="image" src="https://github.com/user-attachments/assets/1cc1abb1-dd1d-4834-9f51-dff7ecef2301" /><br/>

3. Install MCP Inspector and debug the local MCP
```
npx  @modelcontextprotocol/inspector
```
<img width="740" height="488" alt="image" src="https://github.com/user-attachments/assets/94636a94-6794-4e5e-8478-e6fd4dbf1261" /> <br/>

Then launch the local MCP by run:
```
python server.py
```
<img width="1248" height="194" alt="image" src="https://github.com/user-attachments/assets/a46019aa-f146-47b3-8485-d16387f94603" /><br/>

Test the local MCP in MCP Inspector:<br/>
<img width="2483" height="1509" alt="image" src="https://github.com/user-attachments/assets/42f46c0a-f433-491e-99d6-731a201b01a4" /><br/>
You can see the MCP Server can be connected and tool test passed.

