# Build Power Apps with AI | Canvas Apps Authoring MCP Server + Claude Code
Reference:  https://www.youtube.com/watch?v=0MM2BTBiRpc&list=PLTyFh-qDKAiEIVlidnhELx5BusnzlDzkR <br/>

1. Create Canvas App Manually and connect to SharePoint List
2. Enable coauthoring
3. In Github Copilot CLI, type
```
configure canvas mcp
```
<img width="1939" height="735" alt="image" src="https://github.com/user-attachments/assets/3525b62c-88b1-4b6c-9ee3-4a2aea9fff0b" /><br/>
Copy the url from Canvas App Studio <br/>
```
https://make.powerapps.com/e/<Env ID>/canvas?action=edit&template-name=HeaderMainFooter&app-id=%2Fproviders%2FMicrosoft.PowerApps%2Fapps%2Fcbf5df01-5d94-4879-86a8-f6d53d91b31c
```
Ask for confirm<br/>
<img width="1886" height="713" alt="image" src="https://github.com/user-attachments/assets/fb900022-abf4-4d23-a967-c622f2971bd3" /><br/>

Signed in as a different account <br/>
<img width="1917" height="583" alt="image" src="https://github.com/user-attachments/assets/31bd6c53-b274-4cd8-b72a-45b6c555d3c6" /><br/>

4.Check the data source:<br/>
<img width="1838" height="315" alt="image" src="https://github.com/user-attachments/assets/33f66699-4a80-4fb1-85fb-08c5a7e91237" /><br/>

Pull the schema of each data source<br/>
<img width="1830" height="913" alt="image" src="https://github.com/user-attachments/assets/8347a012-7627-45f0-b910-50d1bb95f291" /><br/>

5. Build the App <br/>
```
Please help analyze the data source of this App, actually there are 3 SharePoint list,  and based on these 3 SharePoint List, please plan and help build a Canvas App.
```

6. Once the App is built successfully,  please test the App,  any issue please ask GitHub Copilot fix it:<br/>
<img width="2494" height="1081" alt="image" src="https://github.com/user-attachments/assets/9aa3ff8d-f5d6-4441-b981-1d0a4c6e8061" /><br/>
<img width="2495" height="948" alt="image" src="https://github.com/user-attachments/assets/f4b82268-9d0e-4c7e-92af-6fa5d4368f89" /><br/>
<img width="2489" height="1020" alt="image" src="https://github.com/user-attachments/assets/9ca3ef7a-2a72-430e-8e1f-530c4c98a16d" /><br/>

7. Request suggestion to improve the App <br/>
```
What suggestions do you have to improve the app experience?
```
