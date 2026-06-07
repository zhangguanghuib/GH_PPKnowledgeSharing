# This topic we will talk about this option <br/>
<hr/>
Step 1:  Create a cloud flow,  add this trigger:<br/>
<img width="1400" height="694" alt="image" src="https://github.com/user-attachments/assets/32086842-0990-4cdb-9981-fc7de9dd8410" /><br/>
Choose specific user in my tenant <br/> 
<img width="1425" height="903" alt="image" src="https://github.com/user-attachments/assets/d6515eaf-800c-43a5-91be-732017efa223" /><br/>
And we will add this request body <br/>

```
{
    "type": "object",
    "properties": {
        "userName": {
            "type": "string"
        }
    }
}
```
We can add another compose action:<br/>
<img width="1590" height="499" alt="image" src="https://github.com/user-attachments/assets/9a96a833-fb11-4770-a1cf-b4ca4dc80b39" /><br/>

Save the flow to generate the Http url:<br/>
<img width="1468" height="1112" alt="image" src="https://github.com/user-attachments/assets/eb06dd14-f449-4ee9-9fec-4319925fad8c" />v

The url is
```
https://defaultb0435f53aeb2434e9765c2548b755f.65.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/f9ec8111c54843b7826e603af6b8ff73/triggers/manual/paths/invoke?api-version=1
```
<br/>
<hr/>

Step 2:  Create an App Registraion from Azure Portal <br/>

<img width="1676" height="1529" alt="image" src="https://github.com/user-attachments/assets/1faade4b-c2ae-46f9-9f3e-ef3dea674059" /><br/>

Click "Register" button<br/>

Add API Permission:<br/>
<img width="2415" height="1189" alt="image" src="https://github.com/user-attachments/assets/107e63b2-0223-4769-9f7f-23b844681f33" /><br/>

Click the button "Add Permission"<br/>
<img width="2268" height="1462" alt="image" src="https://github.com/user-attachments/assets/ef4de8e4-9c1a-471f-9c17-d686eab4dc11" /><br/>

Click "Grant admin consent for Contoso"<br/>
<img width="2239" height="1300" alt="image" src="https://github.com/user-attachments/assets/4841c9d1-fd7b-4f77-a7a7-18cbf3b43692" /><br/>

Add Redirect URL->Single Page Application<br/>
```
https://insomnia.rest/oauth/callback
```
And please do remember to tick "ID Tokens and Access Tokens"<br/>
<img width="2446" height="1270" alt="image" src="https://github.com/user-attachments/assets/9a32f2fe-3bfe-40c0-9aa5-437fb034719c" /><br/>
Click "Configure" button<br/>
<img width="2476" height="1394" alt="image" src="https://github.com/user-attachments/assets/37a287d7-d8a8-4134-8eb8-9726f5d17938" /><br/>
Finally it looks like <br/>
<img width="2317" height="1040" alt="image" src="https://github.com/user-attachments/assets/13f353f9-8af3-4a64-a2c2-be1a2a51228c" /><br/>

Adding client secret <br/>
<img width="2470" height="1398" alt="image" src="https://github.com/user-attachments/assets/8b398f38-157b-42b5-a72a-c0cd36471f7a" /><br/>
Please copy and store the client secret value to some place for future use <br/>
<img width="1997" height="1006" alt="image" src="https://github.com/user-attachments/assets/5a9a36bb-42b3-4f3f-95bb-bbb37909cd47" /><br/>
<hr/>

3. Build the HTTP Request to triger the cloud flow<br/>

Open the Application "Insonmia"=>Add a new "Http Request" <br/>
<img width="939" height="860" alt="image" src="https://github.com/user-attachments/assets/4f821b5c-a6dd-4cf4-b983-b00452334b22" /><br/>

Set the Http Method "POST", Copy the Http trigger url as below:<br/>
<img width="2321" height="1226" alt="image" src="https://github.com/user-attachments/assets/d120334b-4b66-4232-949c-ba5b4a728279" /><br/>

Adding the http request headers<br/>
<img width="1873" height="632" alt="image" src="https://github.com/user-attachments/assets/15e59e42-25ba-458e-a21e-d3bac88fe425" /><br/>

Adding the Request Body <br/>
<img width="1856" height="762" alt="image" src="https://github.com/user-attachments/assets/108d7cd2-93a6-4044-83ab-684a5d838189" /><br/>
<hr/>

4. Acquire the access token for "Any user in my tenant"<br/>
Under the "Auth" tab and click "OAuth 2.0" <br/>
<img width="1834" height="1013" alt="image" src="https://github.com/user-attachments/assets/5d329cd4-9076-4b4d-b2de-5ff008cdd3c9" /><br/>

Grant Type:  Implicit <br/>
<img width="1850" height="1052" alt="image" src="https://github.com/user-attachments/assets/4fbb15a6-461b-4128-8414-b35ae165c989" /><br/>

Fill the below fields<br/>
<img width="1873" height="1212" alt="image" src="https://github.com/user-attachments/assets/3b4d1436-5ddf-434f-9b19-fd770163121f" /><br/>

```
Authorization URL: https://login.microsoftonline.com/common/oauth2/authorize?resource=https://service.flow.microsoft.com/
Redirect URL: https://insomnia.rest/oauth/callback
Scope: https://service.flow.microsoft.com//.default
Audience:  https://service.flow.microsoft.com/
```

<hr/>

5. Click "Fetch Token" on the above screen-shot<br/>
<img width="1826" height="1396" alt="image" src="https://github.com/user-attachments/assets/242e740e-b040-416b-ae20-9dc1736f5836" /><br/>
Copy the token, paste to the header: <br/>
<img width="1799" height="635" alt="image" src="https://github.com/user-attachments/assets/57d30a8d-f544-416f-b6fc-9410e8b5575b" /><br/>

<hr/>
6. Trigger the flow by sending HTTP Request <br/>
Click "Send" button<br/>
<img width="2392" height="716" alt="image" src="https://github.com/user-attachments/assets/64f3bd24-2f1f-41e6-a255-9e3e6c43639f" /><br/>

See the response "202 accepted" <br/>
<img width="2392" height="716" alt="image" src="https://github.com/user-attachments/assets/939baac6-686b-42da-9526-4f7094557c34" /><br/>

Go to Power Automate flow, and found the flow is triggered successfully<br/>
<img width="1320" height="940" alt="image" src="https://github.com/user-attachments/assets/f9f2505d-a5fb-492b-a46e-08b6f75c7e01" /><br/>
<img width="1713" height="1175" alt="image" src="https://github.com/user-attachments/assets/d626160e-629e-4951-b9b1-439cc73d1ced" /><br/>
<hr/>




