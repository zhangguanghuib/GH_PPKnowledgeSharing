Now Power Apps Code App support to connect to SharePoint and Azure SQL, not limited to Dataverse<br/>

You can refer to this document to connect to different datasource https://learn.microsoft.com/en-us/power-apps/developer/code-apps/how-to/connect-to-data <br/>

Below are some useful commands <br/>
```
npx degit github:microsoft/PowerAppsCodeApps/templates/vite WorkTrackTrackList01
npm install
npx power-apps init
```
<img width="1642" height="433" alt="image" src="https://github.com/user-attachments/assets/d0820b40-0b73-4b24-b1bc-fa614e1bf376" /><br/>

```
npx power-apps add-data-source
```
<img width="1949" height="986" alt="image" src="https://github.com/user-attachments/assets/7e20e3da-089a-4627-b109-b4ed9155ba56" /><br/>

Start building App with the below prompt:<br/>
```
The SharePoint list is already connected as a data source. read the rules from https://learn.microsoft.com/en-us/power-apps/developer/code-apps/how-to/sharepoint-operations
```
```
Set up a clean, modern SaaS design system: system font stack, a tasteful neutral palette with one accent color, generous spacing, rounded cards, subtle shadows. Build a top app bar titled "Work Tracker"
Render work items from the list in a responsive data table with columns: ID, Title, Category, Progress, Priority, Assigned to, Due date. Show polished skeleton loaders while fetching and a friendly error state if the call fails
```
```
I am only seeing 100 rows returned. Add Pagination - I want data to be loaded in batches of 100 as the user paginates.
```
```
Add sorting and filter options for the Grid columns Title, Progress, Priority and Due Date. All sort and filter actions must be performed server side. Do not perform sort and filter actions on already loaded data.
```
Adding Office 365 User Connector<br/>
<img width="1285" height="351" alt="image" src="https://github.com/user-attachments/assets/79635ff7-cf7b-4fd7-888d-d314b54825de" /><br/>
```
Get user profile photo from Office365Users connector
```
```
Add Full CRUD with a slide-over panel. Add a right-side slide-over panel with a form for all editable fields: Title (required), Description (multiline), Category (multi-select), Progress (select), Priority (select), Start date + Due date (date pickers)
A "+ New work item" button opens the panel empty; clicking a card opens it pre-filled. On save, call create or update. Add a delete action with a confirmation dialog. Show toast notifications for success and error. Also add AssignedTo person column in CRUD forms - load the users from office365Users connector
```

Finally we can run <br/>
```
npm run dev
num run build
npx power-apps push
```
Finally you can see the App is created in VS code :<br/>
<img width="468" height="1281" alt="image" src="https://github.com/user-attachments/assets/c28f4859-3771-487c-8414-8a41260e575c" /><br/>
<img width="2873" height="1431" alt="image" src="https://github.com/user-attachments/assets/17b074f6-152c-471d-8069-a938c8ba9b02" /><br/>

<hr/>

# Trouble shooting:<br/>

In the first step, after you provide the environment id, you may encouter this error:<br/>
```
I followed all these steps. but still get the same error:  Please provide the environment ID:
│ *************
HTTP error status: 404 for GET https://********.19.environment.api.powerplatform.com/powerapps/environment?api-version=1&$filter=name eq '*******': {"error":{"code":"ServiceToServiceEnvironmentNotFound","message":"The environment '*****' could not be found in the tenant '****'."}}, can you please help resolve the issue
```
This is a cache issue because in VS code we logged on different users, you need try these command:
```
pac env list
pac auth list
pac auth create --tenant <your-tenant-id>
# or to target a specific environment directly:
pac auth create --environment <env-id>
```
then  run<br/>
```
npx power-apps init
```
If not work, please try, the reason is:  The @microsoft/power-apps CLI has its own auth system — it does not use pac CLI, Azure CLI, or .IdentityService.<br/>
```
# 1. Wipe the power-apps CLI's own MSAL cache + active-account pointer
npx power-apps logout

# 2. Sign in interactively, pre-filling the right account
npx power-apps login --account guazha@dynamicsftegcr.onmicrosoft.com

# 3. Confirm the active account is the dynamicsftegcr one
npx power-apps auth-status

# 4. Now run init
npx power-apps init
```

# Reference:<br/>
https://learn.microsoft.com/en-us/power-apps/developer/code-apps/how-to/connect-to-data <br/>
https://learn.microsoft.com/en-us/power-apps/developer/code-apps/how-to/sharepoint-operations<br/>
