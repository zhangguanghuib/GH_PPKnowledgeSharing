# How to get active users for Power Automate Premium License

There are 3 ways to get active users for Power Automate Premium License(Because I don't have Premium License, so I use Power Automate Free License as example)

## 1.  From Microsoft Admin Center:

<img width="1886" height="1354" alt="image" src="https://github.com/user-attachments/assets/1b05dfa3-e565-43ec-a646-850f952070d7" /><br/>
<img width="2332" height="1317" alt="image" src="https://github.com/user-attachments/assets/55c993b7-81ef-49c9-83b1-e70792bfd3fc" /><br/>

## 2.  Run Power Shell Script:

```ps
# One-time install
Install-Module Microsoft.Graph -Scope CurrentUser
# Connect with least-privilege scopes
Connect-MgGraph -Scopes "User.Read.All","Directory.Read.All"
# --- 1) Which SKUs are present in this tenant? --
Get-MgSubscribedSku -All |
    Select-Object SkuPartNumber, SkuId,
                  ConsumedUnits,
                  @{n='Enabled'; e={$_.PrepaidUnits.Enabled}} |
    Sort-Object SkuPartNumber
```
