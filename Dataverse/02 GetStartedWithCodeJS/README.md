# Dataverse Web API JavaScript Quickstart

A Parcel-based single-page application that signs in with Microsoft Entra ID and calls the Dataverse Web API `WhoAmI` function.

## Microsoft Entra configuration

The app registration must include:

- Platform: **Single-page application (SPA)**
- Redirect URI: `http://localhost:1234/redirect.html`
- API permission: **Dynamics CRM** > delegated permission `user_impersonation`

The local `.env` is configured for:

- Dataverse: `https://org5efadec2.api.crm.dynamics.com`
- Client ID: `3340c8e2-e921-4f47-9229-22b1dabdeede`
- Tenant ID: `50e627b2-4aa4-4b2f-a97f-a00e384850ca`

The `.env` file is ignored by Git. Use `.env.example` when creating configuration for another environment.

## Run

```powershell
npm install
npm start
```

Open <http://localhost:1234>, select **Sign in**, and then select **Run request**. A successful response displays the current Dataverse user ID.

## Build

```powershell
npm run build
```
