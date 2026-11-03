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

