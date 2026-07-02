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
