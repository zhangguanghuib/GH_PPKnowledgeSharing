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
If you faced the GitHub CLI Sessions logged on to other tenant like MACAPS tenant, you can input these prompt to let the session switch to your desired tenant + environment:
```
please switch to this tenant <Your tenant Id>, power platform environment : <Your Power Platform Environment Id>, account: xxxx@<your domain name>.onmicrosoft.com
```
4. Case study

In this case  I will build a flow that flow will help cancel the flow run got stuck and never stop, so we can use this prompt and ask Power Automate MCP to help build this kind of flow:<br/>
```
https://make.preview.powerautomate.com/environments/<Power Platform Environment Id>/flows/<flow Id>/details,  this power automate flow has many flow runs and keep at running status and never finish,  please help create a new flow to retrieve these running flow runs, and then cancel all these running flows one by one, you can add more filter conditions like : If the flow run start 24 hours ago and still in running status etc.
```
The we run the created flow and found some issue, we may need the Power Automate MCP to help update the created flow with this prompt:<br/>
```
Finally the flow is created successfully,  but the  flow run is failed, https://make.preview.powerautomate.com/environments/<Env Id>/flows/<Flow Id>/runs/<Flow Run Id>, the error message is:,"message":"The workflow '<flow if>' run '<Run Id>' with state 'Succeeded' could not be canceled, because it is not active.","messageTemplate":"WorkflowRunCanNotBeCancelled",  that is when the flow run is retrieved, its status is running, but when call flow cancel action, the status is already successful, so I think in the apply for each loop, befor cancel the flow, we need add another action to retrieve the flow run status, if its status is really running, then cancel it, if the status is failed or successfully, then do nothing but continue for the next flow run.
```
Finally you can see the Power Automate MCP can help us build the Power Automate Flow that can be used to cancel the log running flow runs:<br/>
<img width="1196" height="1797" alt="image" src="https://github.com/user-attachments/assets/1f1d2ca2-dc07-4004-bcb9-dfd82edb7420" /><br/>

5. Test the flow created by MCP:<br/>
<img width="1502" height="1809" alt="image" src="https://github.com/user-attachments/assets/b0034a77-7e84-4130-8c6f-eb289efc049c" /><br/>

Check the original flow, and confirm its long running flow runs got cancelled<br/>
<img width="2165" height="1541" alt="image" src="https://github.com/user-attachments/assets/89b3bfc4-500c-4494-ab58-ea422f73df53" /><br/>

6. Some points we may be careful:<br/>
Please check more details of this connectors<br/>
We need create this connection when send HTTP request to retreive flow run status:<br/>
<img width="1388" height="186" alt="image" src="https://github.com/user-attachments/assets/c4994bc7-e1bb-43b9-abb7-129191e640ee" /><br/>
```
Base resource URL: https://api.flow.microsoft.com
Microsoft Entra ID resource URI: https://service.powerapps.com/
```
<img width="1524" height="1130" alt="image" src="https://github.com/user-attachments/assets/de93bb72-b888-4c6a-a6e2-859f0a199972" /><br/>

7.  For cancel a flow run, we can use the existing connectors<br/>

   Power Automate Management connector to:
  . Get a flow:
  . Cancel a flow。
  . Resubmit a flow
   
  <img width="961" height="996" alt="image" src="https://github.com/user-attachments/assets/44f3a04f-ca34-43f0-b7a2-7bf809e4bf45" />
  




