Trouble-Shooting Power Automate flow connect to Azure Databricks by Service Principal failed<br/>
https://www.youtube.com/watch?v=WkBO5li21lo<br/>

# Issue description:<br/>
In power automate (New designer), when create connection to data bricks,  the connection seems connected successfully, but there is an error in the banner to show there was a problem with the connection, see below:<br/>
<img width="1409" height="710" alt="image" src="https://github.com/user-attachments/assets/985032b1-2e42-4adc-9e5a-a08dd482c2a0" /><br/>

When we go to make.powerautomate.com->Connections->Find the Azure Databricks connection, we can see the connection status is connected:<br/>
<img width="1601" height="935" alt="image" src="https://github.com/user-attachments/assets/fabc53ff-3cdd-4bf3-a1c4-dad7015c52fd" /><br/>

This caused the confusion,  the connection status is connected but in flow designer it shows there is a problem with the connecton<br/>

# Investigation:<br/>

This is a new designer bug, in the new designer no matter the connection is connected successfully or not, it always shows connected.<br/>

The way is we can revert back to the old designer, and create the connection again, and open the network track to monitor the logs, we can see <br/>
<img width="2239" height="1062" alt="image" src="https://github.com/user-attachments/assets/3f16323b-005f-4f48-903b-8c4feaa5ecea" />

From the testconnection we can see the response shows "User not authorized", that is the root cause of the issue<br/>

# Resolution:

We need add the grant service principle permission to the Databricks:<br/>
Step 1: Workspace settings->Identity and settings->Service Principals<br/>
<img width="2191" height="1033" alt="image" src="https://github.com/user-attachments/assets/e8759ab8-d7ce-484a-906e-a72755485f6a" /><br/>

<img width="2125" height="1042" alt="image" src="https://github.com/user-attachments/assets/e1aee00a-62e2-450a-9482-ae797d3ea076" /><br/>

<img width="2174" height="1010" alt="image" src="https://github.com/user-attachments/assets/bcb81f2f-a6f5-4d00-823c-97d1a19ccdaa" /><br/>

Step2: At SQL Warehouse, add service principal<br/>
<img width="2188" height="897" alt="image" src="https://github.com/user-attachments/assets/37521204-8a64-4b21-b3e6-9454ee99053e" /><br/>
<img width="2203" height="1078" alt="image" src="https://github.com/user-attachments/assets/5b5eeab7-d7c0-437c-ae54-306bbd2c15bc" /><br/>
<img width="2228" height="1000" alt="image" src="https://github.com/user-attachments/assets/1ecc0893-883f-4b7f-bcc6-d1a9b2ce1cba" /><br/>

# Verification:

Finally, verify the connection can be connected successfully:<br/>
Go back to the power automate and create the connection again, this time the connection can be connected successfully.<br/>
<img width="2229" height="1016" alt="image" src="https://github.com/user-attachments/assets/482d5764-990c-453e-98c3-67aba27a07c3" /><br/>
Run flow, and it shows run successfully:<br/>
<img width="1933" height="1075" alt="image" src="https://github.com/user-attachments/assets/6576dfd3-5f3a-475c-b564-af121d4d2093" />

