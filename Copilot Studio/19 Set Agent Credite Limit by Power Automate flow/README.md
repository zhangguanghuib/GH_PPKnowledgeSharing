# Set Agent Credit Limit by Power Automate flow
## Step 1:  Go to Power Platform Admin Center to set credit limit for one agent, and get the network trace
<img width="2494" height="1356" alt="image" src="https://github.com/user-attachments/assets/33468e55-9357-4c94-ba88-5fee664bbdfa" /><br/>

Get the format of API call <br/>
<img width="2459" height="620" alt="image" src="https://github.com/user-attachments/assets/a7fe4355-a471-4741-a228-c1cffc07e2b3" /><br/>

Payload of this request is in the format:<br/>
<img width="2440" height="935" alt="image" src="https://github.com/user-attachments/assets/89f276d6-c870-46f6-8056-728f8cd188b0" /><br/>

<img width="2418" height="1198" alt="image" src="https://github.com/user-attachments/assets/47da76a6-a837-432a-bd90-48995a9b1444" /><br/>

Request URL:

```
https://<TenantId Split by last two chars>.tenant.api.powerplatform.com/licensing/environments/<Environment Id>/entitlements/MCSMessages/resources/<BotId>/threshold
```

```json
{
    "resourceId": "<botId>",
    "entitlementId": "MCSMessages",
    "environmentId": "<environmentid>",
    "limit": 200,
    "stopIfOverCapacity": true,
    "notifyIfOverCapacity": true,
    "notificationThreshold": 90,
    "resourceConsumption": 95.92
}
```

## Step 2:  Create Power Automate flow to set Agents Credit Limit<br/>
<img width="518" height="781" alt="image" src="https://github.com/user-attachments/assets/925b7759-7f2e-4fa3-a070-db86e9f46b43" /><br/>

