<h1> Disable user access Copilot Studio Portal </h1> <br/>
# https://learn.microsoft.com/en-us/microsoft-copilot-studio/requirements-licensing
# https://learn.microsoft.com/en-us/entra/identity/users/directory-self-service-signup
# https://learn.microsoft.com/en-us/microsoft-copilot-studio/admin-block-viral-signups
# https://learn.microsoft.com/en-us/microsoft-copilot-studio/admin-data-loss-prevention?tabs=webApp

Step 1:  Please run the below powershell script in Power Shell 7: <br/>
<img width="300" height="127" alt="image" src="https://github.com/user-attachments/assets/d5811a9d-b450-4692-a40f-aac8128b01f9" /><br/>

If you run it in the old version Power Shell, it will fail: <br/>

```ps
Import-Module Microsoft.Graph.Identity.SignIns
Connect-MgGraph -Scopes "Policy.ReadWrite.Authorization"
$param = @{
    allowedToSignUpEmailBasedSubscriptions = $false
    allowEmailVerifiedUsersToJoinOrganization = $false
    allowAdHocSubscriptions = $false
}
Update-MgPolicyAuthorizationPolicy -BodyParameter $param
```
<img width="1199" height="512" alt="image" src="https://github.com/user-attachments/assets/c6dae4e8-1915-4b5a-a49b-3e8f513ffbd1" /><br/>

Step 2:  Install the below Power Shell Modules<br/>
```ps
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Install-Module PackageManagement -Force -Scope CurrentUser -AllowClobber -SkipPublisherCheck
Install-Module PowerShellGet -Force -Scope CurrentUser -AllowClobber -SkipPublisherCheck
Install-Module Microsoft.PowerShell.PSResourceGet -Scope CurrentUser -Force -AllowClobber -SkipPublisherCheck
Import-Module  Microsoft.PowerShell.PSResourceGet
```

Step 3: Install Commerce related module <br/>
```ps
Install-PSResource -Name MSCommerce -Scope CurrentUser -Reinstall -TrustRepository
 Import-Module MSCommerce
```

Step 4: List <br/>
```ps
Connect-MSCommerce
Get-MSCommerceProductPolicies -PolicyId AllowSelfServicePurchase
```
<img width="1473" height="872" alt="image" src="https://github.com/user-attachments/assets/af4f6950-d947-4afb-b473-c9e0341f49fe" /><br/>

Step 5 : Disable self-service purchase for a product <br/>
```
Get-MSCommerceProductPolicies -PolicyId AllowSelfServicePurchase
```

Step 6:  In Microsoft Admin Center, try this:<br/>

<img width="1318" height="1303" alt="image" src="https://github.com/user-attachments/assets/9e5fc2b3-95d0-425e-a93a-55950e1b5155" /><br/>

Step 7:  Check Copilot Studio Portal<br/>
<img width="2667" height="1263" alt="image" src="https://github.com/user-attachments/assets/89c5c5d7-6d3b-47e6-8a59-5f256717d75c" /><br/>
