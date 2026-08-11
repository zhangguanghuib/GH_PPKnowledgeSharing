# Issue,  file can't be uploaded to Storage Account Container:

## See the issue
Open the Storage Account, find container, and click it to open:<br/>
<img width="2466" height="1217" alt="image" src="https://github.com/user-attachments/assets/9dc69c15-31b3-4a8f-a239-59aced09cbdb" /><br/>
<img width="1967" height="858" alt="image" src="https://github.com/user-attachments/assets/86addbde-2c89-456d-adf1-1d35b568a4fb" /><br/>

## Resolution:

1. Go back to the Storage Account, add tags:
```
Name: SecurityControl
Value: Ignore
```

<img width="377" height="387" alt="image" src="https://github.com/user-attachments/assets/1eeef0b5-e22a-4c3b-87db-4dafe3bc5d47" /><br/>
<img width="2415" height="1353" alt="image" src="https://github.com/user-attachments/assets/599baa89-1c42-4a52-8f4e-6a77287bdba9" /><br/>

2. Enable Public Access:
<img width="1809" height="1176" alt="image" src="https://github.com/user-attachments/assets/e23208b1-6866-4e2a-8381-246858cfbd58" /><br/>
The IP need manually be added<br/>
<img width="1868" height="1342" alt="image" src="https://github.com/user-attachments/assets/5a1e526d-755f-4d77-a60e-eda025d44d91" /><br/>
Double check public access is enabled<br/>
<img width="2086" height="1169" alt="image" src="https://github.com/user-attachments/assets/0f0221d9-739e-405d-830c-03216ffa544d" /><br/>

3. Add role:
   <img width="2400" height="1182" alt="image" src="https://github.com/user-attachments/assets/3abd2570-e017-4cf3-8827-8610f322449d" /><br/>
   Search this role "Storage Blob Data Contributor"<br/>
   <img width="2466" height="1420" alt="image" src="https://github.com/user-attachments/assets/695222e8-5f25-4408-8681-56c0b2ef28cc" /><br/>
   Select users<br/>
   <img width="1934" height="1385" alt="image" src="https://github.com/user-attachments/assets/f813851a-e66c-4b34-bc08-d5e523538eae" /><br/>
   Click next until "Review and Assign"<br/>
## Verify
<img width="2481" height="1302" alt="image" src="https://github.com/user-attachments/assets/47af7748-6063-4d9b-852d-d5b357eeb8b8" /><br/>
You can see the files can be uploaded successfully<br/>
<img width="2419" height="517" alt="image" src="https://github.com/user-attachments/assets/7ed459b9-ea15-4204-aa6e-956d9c2791ed" /><br/>

## Additional information

This is the vNet I created, nothing special:<br/>
<img width="2076" height="1155" alt="image" src="https://github.com/user-attachments/assets/056cb9f9-06c6-416b-b03c-5efbb2ec6cf4" />
