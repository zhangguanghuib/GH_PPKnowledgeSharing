# Build Power Apps with AI | Canvas Apps Authoring MCP Server + Claude Code
Reference:<br/>
https://www.youtube.com/watch?v=0MM2BTBiRpc&list=PLTyFh-qDKAiEIVlidnhELx5BusnzlDzkR <br/>
https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/create-canvas-external-tools<br/>

1. Create SharePoint Lists:
  <img width="2172" height="432" alt="image" src="https://github.com/user-attachments/assets/56606372-c08f-40e5-a8bc-ac4b7970ab73" /><br/>
  <img width="1436" height="365" alt="image" src="https://github.com/user-attachments/assets/2b1a464e-0d56-4128-8d70-1c93aae2a265" /><br/>
  <img width="1073" height="258" alt="image" src="https://github.com/user-attachments/assets/ed9629de-62f3-437e-ac51-f749d1b0378e" /><br/>

2. Create Canvas App Manually and connect to SharePoint List
<img width="2054" height="920" alt="image" src="https://github.com/user-attachments/assets/825f40d2-b522-4617-b1fd-a960f19fb156" /><br/>

3. Enable coauthoring
<img width="1334" height="859" alt="image" src="https://github.com/user-attachments/assets/de62aa4b-05ff-444d-96e6-9ed466edfb09" /><br/>

4. In Github Copilot CLI, type
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
8.  Add a new column "Admin Comments" to the sugggestions list <br/>

<img width="2103" height="333" alt="image" src="https://github.com/user-attachments/assets/9da5855a-f217-446d-a241-eb07255d6f9a" />

```
I added a new column called admin comments in suggestions list. This should be what the admin can optionally add when updating the Status
```
9. Add two connectors:<br/>

<img width="1999" height="1185" alt="image" src="https://github.com/user-attachments/assets/9c9f6856-9eb9-47fd-83cc-c4ffffec7c47" /><br/>
```
In the ideas gallery - show the created by users Image using office365Users connector - when admin changes the status - notify the idea creator via email of the status change and also include the admin comments. Make the email look visually appealing!
```
