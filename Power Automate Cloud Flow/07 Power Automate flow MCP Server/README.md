# Power Automate Server MCP Server

1. Check the document and install Power Automate MCP Server
https://github.com/microsoft/power-platform-skills/tree/main/plugins/power-automate <br/>

Open GitHub CLI and install the Power Automate MCP Server<br/>
```
/plugin marketplace add microsoft/power-platform-skills
/plugin install power-automate@power-platform-skills
```
And then input
```
/skills list
```
We can the skills the Power Automate MCP Server provided:<br/>
<img width="2388" height="643" alt="image" src="https://github.com/user-attachments/assets/23da617c-9e8f-42ea-8ccd-d1ed54480b55" /><br/>
Then input
```
power automate mcp server
```
You can see the CLI is asking what you are going to do:<br/>
<img width="1004" height="575" alt="image" src="https://github.com/user-attachments/assets/0744cdf7-2c1d-42d1-889d-37af2a2c4a4e" /><br/>

2. Double check the Power Automate MCP Server is installed<br/>
```
/mcp list
```
<img width="473" height="517" alt="image" src="https://github.com/user-attachments/assets/a3578257-f956-4136-8cf4-03d122cb2b00" /><br/>

3. Logon to the current Power Platform environment<br/>
If you faced the GitHub CLI Sessions logged on two other tenant like MACAPS tenant, you can input these prompt to let the session switch to your desired tenant + environment:
```

```
