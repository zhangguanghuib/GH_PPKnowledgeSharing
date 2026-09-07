import {
  InteractionRequiredAuthError,
  PublicClientApplication,
} from "@azure/msal-browser";
import "dotenv/config";

const config = {
  baseUrl: process.env.BASE_URL,
  clientId: process.env.CLIENT_ID,
  tenantId: process.env.TENANT_ID,
  redirectUri: process.env.REDIRECT_URI,
};

const msalConfig = {
  auth: {
    clientId: config.clientId,
    authority: `https://login.microsoftonline.com/${config.tenantId}`,
    redirectUri: config.redirectUri,
    postLogoutRedirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: "sessionStorage",
    storeAuthStateInCookie: true,
  },
};

const msalInstance = new PublicClientApplication(msalConfig);
const container = document.getElementById("container");
const loginButton = document.getElementById("loginButton");
const logoutButton = document.getElementById("logoutButton");
const buttonContainer = document.getElementById("buttonContainer");
const connectionStatus = document.getElementById("connectionStatus");
const connectionStatusText = document.getElementById("connectionStatusText");
const whoAmIButton = document.getElementById("whoAmIButton");

function renderSession(account) {
  const isSignedIn = Boolean(account);
  loginButton.classList.toggle("hidden", isSignedIn);
  logoutButton.classList.toggle("hidden", !isSignedIn);
  buttonContainer.classList.toggle("disabled", !isSignedIn);
  connectionStatus.dataset.state = isSignedIn ? "online" : "offline";
  connectionStatusText.textContent = isSignedIn ? account.name ?? account.username : "Not connected";
}

function renderMessage(title, detail, state = "success") {
  container.replaceChildren();

  const label = document.createElement("p");
  label.className = "response-label";
  label.textContent = state === "error" ? "Request failed" : "Response received";

  const heading = document.createElement("p");
  heading.className = `response-title ${state}`;
  heading.textContent = title;

  const value = document.createElement("code");
  value.className = "response-value";
  value.textContent = detail;

  container.append(label, heading, value);
}

async function logIn() {
  const request = {
    scopes: ["User.Read", `${config.baseUrl}/user_impersonation`],
  };

  try {
    const response = await msalInstance.loginPopup(request);
    msalInstance.setActiveAccount(response.account);
    renderSession(response.account);
  } catch (error) {
    renderMessage("Unable to sign in", error.message, "error");
  }
}

async function logOut() {
  const account = msalInstance.getActiveAccount();

  try {
    await msalInstance.logoutPopup({
      account,
      mainWindowRedirectUri: window.location.origin,
    });
    msalInstance.setActiveAccount(null);
    renderSession(null);
    renderMessage("Signed out", "The local Dataverse session has been cleared.");
  } catch (error) {
    renderMessage("Unable to sign out", error.message, "error");
  }
}

async function getToken() {
  const request = {
    account: msalInstance.getActiveAccount(),
    scopes: [`${config.baseUrl}/.default`],
  };

  try {
    const response = await msalInstance.acquireTokenSilent(request);
    return response.accessToken;
  } catch (error) {
    if (error instanceof InteractionRequiredAuthError) {
      const response = await msalInstance.acquireTokenPopup(request);
      return response.accessToken;
    }

    throw error;
  }
}

async function whoAmI() {
  const token = await getToken();
  const response = await fetch(`${config.baseUrl}/api/data/v9.2/WhoAmI`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
      Accept: "application/json",
      "OData-Version": "4.0",
      "OData-MaxVersion": "4.0",
    },
  });

  if (!response.ok) {
    throw new Error(`${response.status} ${response.statusText}`);
  }

  return response.json();
}

loginButton.addEventListener("click", logIn);
logoutButton.addEventListener("click", logOut);
whoAmIButton.addEventListener("click", async () => {
  whoAmIButton.disabled = true;
  whoAmIButton.textContent = "Running...";

  try {
    const response = await whoAmI();
    renderMessage("Connected to Dataverse", `User ID: ${response.UserId}`);
  } catch (error) {
    renderMessage("Unable to call WhoAmI", error.message, "error");
  } finally {
    whoAmIButton.disabled = false;
    whoAmIButton.textContent = "Run request";
  }
});

async function initializeApp() {
  await msalInstance.initialize();
  const cachedAccount = msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0] ?? null;
  if (cachedAccount) {
    msalInstance.setActiveAccount(cachedAccount);
  }
  renderSession(cachedAccount);
}

initializeApp().catch((error) => {
  renderMessage("Unable to initialize authentication", error.message, "error");
});
