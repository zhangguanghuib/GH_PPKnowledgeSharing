# 10 Get Flow Run Details by Invoke an HTTP request by HTTP with Microsoft Entra Id Preauthorize

## Step 01, create two connections for this connector "HTTP with Microsoft Entra ID (preauthorized)"
<img width="1712" height="1115" alt="image" src="https://github.com/user-attachments/assets/e930883d-ef40-4408-b509-fcbb0bf54526" /><br/>
<img width="1484" height="990" alt="image" src="https://github.com/user-attachments/assets/5bde2b1b-2ae3-4915-8ebd-9b866ce338a8" /><br/>

The complete flow run design is as below:<br/>
<img width="535" height="1308" alt="image" src="https://github.com/user-attachments/assets/f30b6327-1164-45fb-81be-c7e1a10ccdf4" /><br/>

## Step 02, check flow action details:
1. Get environment id:
   <img width="1484" height="756" alt="image" src="https://github.com/user-attachments/assets/c9ff17b0-ff78-4cce-83a2-476f8754b4aa" /><br/>

2. Get one flow's all run history:
   <img width="1477" height="1265" alt="image" src="https://github.com/user-attachments/assets/c59128e6-4696-4701-a686-fa4a9b9137ae" /><br/>

3. Apply to each:
   <img width="1590" height="1357" alt="image" src="https://github.com/user-attachments/assets/158730c0-d5d8-4ee9-af6d-74f9f139ae35" /><br/>

4. Get Flow Run Id:
   <img width="1459" height="1240" alt="image" src="https://github.com/user-attachments/assets/ef845e44-4857-4d2b-9e01-2288c347b7a9" /><br/>

5. Get Flow Run Name:
   <img width="1680" height="1304" alt="image" src="https://github.com/user-attachments/assets/55d05a2f-6cbf-48cb-99a0-3e322e38e10b" /><br/>

6. Get one flow run details
   <img width="1803" height="1312" alt="image" src="https://github.com/user-attachments/assets/0cfa5102-790d-43a2-b4da-dd135afe95f4" /><br/>

## Step 03, how to create two connections:
1. For getting run history of a flow, you can get he request url from:<br/>
  <img width="2362" height="1025" alt="image" src="https://github.com/user-attachments/assets/3e9ed548-be44-45ee-8b2f-a6efadd48f69" /><br/>
  
  ```
   In power automate flow, if I want to create a connection for connector "HTTP with Microsoft Entra ID (preauthorized)" to call this below request, can you please show me how can I create the connection especially how to set the two values for Base Resource URL and Microsoft Entra ID Resource URI (Application ID URI) ? 
  ```
  <img width="1016" height="395" alt="image" src="https://github.com/user-attachments/assets/4fe28fcf-ac0d-4284-9e07-fed17c0b883e" /><br/>

  2. For one single flow run details, you can try:
     <img width="1078" height="451" alt="image" src="https://github.com/user-attachments/assets/1faaf682-8f1a-418f-ac90-2950560374b5" /><br/>

