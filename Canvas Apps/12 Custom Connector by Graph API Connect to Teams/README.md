# Build a custom connector to connect Teams by Graph API

Steps:
# 1. Create App Registration
   <li> API Permissions -> Microsoft Graph -> Delegated Permission</li>
      <img width="2415" height="1213" alt="image" src="https://github.com/user-attachments/assets/ad86c985-1d5f-4c9a-b546-bca73ba5edfc" /><br/>
      <li> Presence.Read.All </li>
      <li> User.Read</li>
      These two permissions are mandatory for this connector
    <li>Create a new Client Secret<br/></li>
      <img width="1682" height="790" alt="image" src="https://github.com/user-attachments/assets/c404480a-5d8b-466c-92e5-1633d4397699" /><br/>
   <li> Redirect URI: Leave it empty and provided during Custom Connector Creation<br/></li>
      <img width="2039" height="971" alt="image" src="https://github.com/user-attachments/assets/cd300eb7-94e4-44f8-9cd7-61c0ffe3ac70" /><br/>

# 2. Create Custom Connector

<li>Step 1. Create Custom Connector from Blank<br/></li>
<img width="1795" height="378" alt="image" src="https://github.com/user-attachments/assets/d2981845-e5a6-4b2a-9c1a-e0c025553f80" /><br/>
<img width="820" height="286" alt="image" src="https://github.com/user-attachments/assets/368778e2-e3c6-4af8-97bf-c88d966aac47" /><br/>
<img width="1793" height="1263" alt="image" src="https://github.com/user-attachments/assets/9449405d-6587-45ea-a748-5510005bddfd" /><br/>

<li>Security</li>
<img width="1604" height="1204" alt="image" src="https://github.com/user-attachments/assets/0773e083-93a2-47ba-a185-1bb34e131396" /><br/>
<img width="1120" height="503" alt="image" src="https://github.com/user-attachments/assets/c2a255ca-bdf7-4e5f-8a88-20cc74f5386f" /><br/>

<li>Create a new Operation<li>
<img width="2495" height="1394" alt="image" src="https://github.com/user-attachments/assets/66232396-5fc8-4c72-b8ec-a1888b25343c" /><br/>

```
{
   "value":
         [
            {"id":"xxx","availability":"Available","activity":"Available"}
         ]
}
```

Click "Create Custom Connector", and then go back to Security Table and find and copy the Redirect URL:<br/>
<img width="1150" height="426" alt="image" src="https://github.com/user-attachments/assets/95f81ca5-2c2c-483a-9aec-3b1ca3615af2" /><br/>
Fill the Redirect URl back to the App Registration<br/>
<img width="2062" height="962" alt="image" src="https://github.com/user-attachments/assets/13f6b342-23f2-4bdd-99fc-b03d686d6c8d" /><br/>

# 3. Test Connector

Input this:
```
{"ids":["f********************"]}
```
<img width="1036" height="1002" alt="image" src="https://github.com/user-attachments/assets/d1fe9074-7cc2-4e84-85c6-b4099d1da066" /><br/>
