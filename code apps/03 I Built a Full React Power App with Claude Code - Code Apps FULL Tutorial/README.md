# https://www.youtube.com/watch?v=EzBXiooDKbI\&t=600s <br/>

<img width="1428" height="746" alt="image" src="https://github.com/user-attachments/assets/49aaec14-87be-430d-9509-dcd9dc9796c2" /><br/>

Code Apps is enabled for Power Platform Environment<br/>
<img width="1669" height="533" alt="image" src="https://github.com/user-attachments/assets/3c51a296-b0b1-4f2d-8021-46641440ba5a" /><br/>

1. Input the below command<br/>
```
npx degit microsoft/PowerAppsCodeApps/templates/starter [your-project-name]
npx degit microsoft/PowerAppsCodeApps/templates/starter GH-Kanban-Board
```

<img width="1422" height="887" alt="image" src="https://github.com/user-attachments/assets/e0a05b07-6870-4d58-8e46-409680d6883d" /><br/>

2. Then run <br/>
```
npm install
```

<img width="1975" height="1141" alt="image" src="https://github.com/user-attachments/assets/f4391b7a-8af9-438c-b999-c09758fbeb38" /><br/>

3. Run:<br/>
```
pac auth create
```

<img width="1887" height="1385" alt="image" src="https://github.com/user-attachments/assets/2697b32b-5e28-4631-b1d0-db194f7653a1" /><br/>

4. Run:<br/>
```
pac org list 
```
5. Run command:<br/>
```
pac org select --environment [environment-id]
```

<img width="1746" height="212" alt="image" src="https://github.com/user-attachments/assets/0b94f1fd-2c04-412f-8495-dceaf5d1b839" /><br/>

6. Run the command <br/>
```
pac code init --displayname 'GH Kanban Board'                                                         
```

7. Run command:
```
npm run dev
```
See the App Starts<br/>
<img width="2264" height="1283" alt="image" src="https://github.com/user-attachments/assets/d7ec4822-692e-4c72-a729-6b86aa0f9f87" /><br/>


10. RUn:
```
npm run build
```

11. run
```
npx power-apps push
```
