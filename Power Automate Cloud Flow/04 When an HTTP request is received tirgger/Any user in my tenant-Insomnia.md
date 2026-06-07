# This topic we will talk about this option <br/>
<hr/>
Step 1:  Create a cloud flow,  add this trigger:<br/>
<img width="1624" height="985" alt="image" src="https://github.com/user-attachments/assets/dff85a14-23a4-4307-9170-e276d0781e49" /><br/>
<img width="1538" height="979" alt="image" src="https://github.com/user-attachments/assets/8aa3f367-59fc-43ba-8ffc-40a29419a3ee" /><br/>
And we will add this request body <br/>
<img width="1476" height="1005" alt="image" src="https://github.com/user-attachments/assets/512ab477-92f7-459a-8e1a-e71f1e4e6c4a" /><br/>

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

