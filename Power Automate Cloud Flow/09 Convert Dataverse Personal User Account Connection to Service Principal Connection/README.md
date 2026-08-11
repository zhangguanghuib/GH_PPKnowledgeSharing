# 1. Convert User Account Dataverse connection to Service Principal connection
```ps
.\Update-DataverseConnectionToServicePrincipal.ps1 -EnvironmentId "EnvironmentId" -ConnectionId "<ConnectionId>" -TenantId "<TenantId>" -ApplicationId "<App Registration Client Id>"
```

# 2. Check a connection is User Account connection or Service Principal Connection
```ps
.\Test-PowerPlatformConnectionIdentity.ps1  -TenantId "<TenantId>" -EnvironmentId "<EnvironmentId>" -ConnectionId "<ConnectionId>"
```
This means the connection is "Service Principal"<br/>
<img width="956" height="362" alt="image" src="https://github.com/user-attachments/assets/d7214a45-08f1-4c9b-971e-143f6fc1789e" /><br/>

This means the connection is a "User Account" connection <br/>
<img width="988" height="384" alt="image" src="https://github.com/user-attachments/assets/134396f4-a17c-4d1d-82e0-a69c4d3634c7" /><br/>

# 3. How to create a service principal connection<br/>
```
Follow these steps for the Global Azure cloud.

1. Create the Entra application

Open Microsoft Entra admin center.
Go to Identity → Applications → App registrations.
Select New registration.
Enter a descriptive name, such as PowerAutomate-Dataverse-Production.
Select Accounts in this organizational directory only.
Select Register.
On the Overview page, record:
Application (client) ID
Directory (tenant) ID
You generally do not need to add delegated Dataverse API permissions. Dataverse authorization is provided through the Application User and its security roles.

2. Create a client secret

Open the app registration.
Select Certificates & secrets → Client secrets.
Select New client secret.
Set a description and expiration according to your organization's policy.
Select Add.
Record the secret's Value immediately.
Use the Value, not the Secret ID. Store it in a secure secret store and plan for rotation before expiration.

3. Create the Dataverse Application User

The service principal must be added separately to every Dataverse environment it needs to access.

Open Power Platform admin center.
Select Manage → Environments.
Select the target environment.
Select Settings → Users + permissions → Application users.
Select New app user.
Select Add an app.
Search using the Application ID or application name.
Select the application and then Add.
Select the appropriate Business unit.
Select the edit icon beside Security roles.
Assign the required security role.
Select Save, then Create.
For production, use a custom least-privilege security role. The role must grant all operations the flow requires, for example:

Create, Read, and Write on the target table
Append on the source table
Append To on related tables
Access to referenced lookup tables
Appropriate user, business-unit, or organization scope
You can temporarily use System Administrator to confirm configuration, but replace it with a restricted role afterward.

4. Create the service-principal connection

Open Power Automate.
Select the correct environment.
Open or create a cloud flow.
Add a Microsoft Dataverse trigger or action.
Open its connection selector.
Select Add new connection.
Select Connect with service principal instead of normal Sign in.
Enter:
Connection name
Client ID
Client secret
Tenant ID
Select Create.
Select the new connection for the Dataverse action or trigger.
If Connect with service principal is not shown from the general Connections page, create it from the connection selector on a Dataverse action inside the cloud-flow designer.

5. Use it in a solution

For production flows, use a connection reference:

Open the flow from its solution.
Open the Dataverse connection reference.
Select the new service-principal connection.
Save the connection reference.
Open and save the flow.
Turn the flow off and back on if the old connection remains cached.
Verify every Dataverse trigger and action uses the intended reference.

Changing a connection reference affects components using that reference. Review all dependent flows before changing a shared production reference.
```

