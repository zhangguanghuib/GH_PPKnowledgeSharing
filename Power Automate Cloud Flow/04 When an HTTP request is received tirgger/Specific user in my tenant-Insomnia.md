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
<img width="1610" height="468" alt="image" src="https://github.com/user-attachments/assets/b0cc7eb9-ba15-40df-8d57-8508f61b848a" /><br/>

Save the flow to generate the Http url:<br/>
<img width="1861" height="955" alt="image" src="https://github.com/user-attachments/assets/f99b7948-b6c0-4bb6-9f4b-769b5c51fd20" /><br/>
The url is
```
https://defaultb0435f53aeb2434e9765c2548b755f.65.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/fdfc6a2e6eed4a1796999b0b63612ac2/triggers/manual/paths/invoke?api-version=1
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
<img width="2307" height="1077" alt="image" src="https://github.com/user-attachments/assets/ab95c59b-eab7-4a8f-afde-945aa35461e9" /><br/>

Adding the http request headers<br/>
<img width="1794" height="616" alt="image" src="https://github.com/user-attachments/assets/90024209-c38e-46eb-a2e7-846918102d81" /><br/>

Adding the Request Body <br/>
<img width="1776" height="613" alt="image" src="https://github.com/user-attachments/assets/4c2962ee-400b-4402-bdee-815d751d2278" /><br/>
<hr/>

4. Acquire the access token for "Any user in my tenant"<br/>
Under the "Auth" tab and click "OAuth 2.0" <br/>
<img width="1765" height="957" alt="image" src="https://github.com/user-attachments/assets/6b7e2cae-052e-45f5-9eda-d0fac47e3e45" /><br/>

Grant Type:  Client Credentials <br/>
<img width="1768" height="1140" alt="image" src="https://github.com/user-attachments/assets/92c82897-cb91-43dd-b849-901c96502b2f" /><br/>

Get the OAuth 2.0 token endpoint (v2)<br/>
<img width="2319" height="742" alt="image" src="https://github.com/user-attachments/assets/d038889f-2b8a-40ed-aab5-4e76cbbc0f6c" />

<img width="1835" height="1328" alt="image" src="https://github.com/user-attachments/assets/f87def74-f5bd-439f-829b-82c3d30a3c59" />

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




