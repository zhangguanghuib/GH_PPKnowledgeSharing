# Integrated Ali DingTalk with Copilot Studio AI Agent 

## 1. Create a Copilot Studio AI Agent
<img width="1581" height="1383" alt="image" src="https://github.com/user-attachments/assets/2ea631a4-11bc-4f33-bfd5-462fab9999e6" /><br/>
Go to Settings->Security->Web channel security<br/>
<img width="2021" height="726" alt="image" src="https://github.com/user-attachments/assets/91aae21c-8fa9-4762-8382-5f32d4a5f1ee" /><br/>
Then get the Direct Line API Endpoint<br/>
<img width="2407" height="1186" alt="image" src="https://github.com/user-attachments/assets/1b88c736-52aa-482c-90a5-6188083dd974" /><br/>

2. Go to https://open-dev.dingtalk.com/ (钉钉开发者平台)
   创建应用：<br/>
   <img width="2456" height="947" alt="image" src="https://github.com/user-attachments/assets/13d025af-9386-45bc-86d5-720ca06cd3cc" /><br/>
   <img width="2452" height="837" alt="image" src="https://github.com/user-attachments/assets/67647007-00c8-4e22-9167-6efc679f5198" /><br/>
   Configure the mode of message receive<br/>
   <img width="1771" height="1467" alt="image" src="https://github.com/user-attachments/assets/6fdc356e-23fb-4463-9fab-e1ac4ed14a5f" /><br/>
   Copy "Client Id" and "Client Secret Id" for future use:<br/>
   <img width="1921" height="1073" alt="image" src="https://github.com/user-attachments/assets/c1d51268-2026-4b4c-9fc0-2bea9fc73d23" /><br/>

3. Write the below code:
   ```
   DINGTALK_CLIENT_ID=
   DINGTALK_CLIENT_SECRET=
   # Set DIRECTLINE_SECRET or COPILOT_TOKEN_ENDPOINT; the secret takes priority.
   DIRECTLINE_SECRET=
    COPILOT_TOKEN_ENDPOINT=
   ```




