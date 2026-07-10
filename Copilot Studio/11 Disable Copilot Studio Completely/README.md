# https://learn.microsoft.com/en-us/microsoft-copilot-studio/requirements-licensing
# https://learn.microsoft.com/en-us/microsoft-copilot-studio/admin-block-viral-signups
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



