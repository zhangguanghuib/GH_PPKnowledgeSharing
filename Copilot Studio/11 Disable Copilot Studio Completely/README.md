# https://learn.microsoft.com/en-us/microsoft-copilot-studio/requirements-licensing
# https://learn.microsoft.com/en-us/microsoft-copilot-studio/admin-block-viral-signups

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



