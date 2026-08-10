# This article shows to how configure Copilot Studio Agent and embedded it into a html page.

## Scenario #1,  the agent is public access without authentication

1. Go to Copilot Studio Agent => Settings => Authentication => No Authentication <br/>
<img width="1855" height="957" alt="image" src="https://github.com/user-attachments/assets/9c60ba00-d445-495a-8712-996a41a93474" />

2. In the same Copilot Studio Agent =>Channels => Direct
<img width="1629" height="1232" alt="image" src="https://github.com/user-attachments/assets/dd1c6b0c-a52e-452e-ace1-6897a8162ba0" /><br/>
Copy the "Token Endpoint":<br/>
<img width="2424" height="975" alt="image" src="https://github.com/user-attachments/assets/81326106-98fa-4b0a-9723-be890f1e4cfc" /><br/>

Paste the Token Endpoint to the html page, the placeholder as:<br/>
<img width="2409" height="1229" alt="image" src="https://github.com/user-attachments/assets/9c30d2e9-98f7-4e51-8ebb-f368c8f4fb93" /><br/>

3. Test the bot inside the webpage<br/>
Click the bot icon<br/>
<img width="1899" height="1138" alt="image" src="https://github.com/user-attachments/assets/f5201ac3-c0b1-480f-86b3-44a56a279eef" /><br/>
You can see the bot work properly:<br/>
<img width="2473" height="1367" alt="image" src="https://github.com/user-attachments/assets/1cc62c3b-9471-4a38-8e72-7d3820b62d66" /><br/>

## Scenario #2,  the agent is not public access but need manual authentication

1. Go to Copilot Studio Agent => Settings => Authentication => Manual Authentication <br/>

<img width="1727" height="1396" alt="image" src="https://github.com/user-attachments/assets/d63b2a3e-358d-4fd2-bf02-643a175890f1" />

2. In the same Copilot Studio Agent =>Channels => Direct
<img width="2428" height="1167" alt="image" src="https://github.com/user-attachments/assets/c5e07429-bbd1-4526-b3de-d1ed54919fc8" /><br/>
Copy the "Token Endpoint":<br/>
<img width="2477" height="1401" alt="image" src="https://github.com/user-attachments/assets/56077f9d-5c34-4108-aeb3-46749b7b8325" /><br/>

Paste the Token Endpoint to the html page, the placeholder as:<br/>
<img width="2371" height="1415" alt="image" src="https://github.com/user-attachments/assets/a814fcd9-8d62-471d-a64a-99e57d2156df" /><br/>

3. Test the bot inside the webpage<br/>
Click the bot icon<br/>
<img width="2466" height="1395" alt="image" src="https://github.com/user-attachments/assets/ea31807a-58ec-4f4a-a86e-29d7dbd93dcb" /><br/>
Please click "Logon" button <br/>
<img width="2462" height="1389" alt="image" src="https://github.com/user-attachments/assets/37538390-1f39-49bb-804a-1a5924ff7bbe" /><br/>

Pick Account to Sign-in:<br/>
<img width="613" height="800" alt="image" src="https://github.com/user-attachments/assets/41ab6f9d-c2d2-4360-bfad-0c5f5f4c583b" />

Copy the code:<br/>
<img width="1009" height="279" alt="image" src="https://github.com/user-attachments/assets/16bddce5-1b61-4a80-be81-cdd64d0261e7" /><br/>

Paste the code to the chatbot<br/>
<img width="777" height="1206" alt="image" src="https://github.com/user-attachments/assets/063392b9-3d71-4afe-bfb4-1f31b069cd30" /><br/>
Then click send<br/>
You can see the bot work properly:<br/>
<img width="2446" height="1411" alt="image" src="https://github.com/user-attachments/assets/3f9f9b7c-a2a9-4626-8000-8df3c9cf07f0" />
<br/>

## Reference:
1. All the code can be found from:
   https://github.com/zhangguanghuib/GH_PPKnowledgeSharing/blob/main/Copilot%20Studio/14%20Embedded%20Copilot%20Agent%20into%20website%20page/Code/testManualAuthenticationBot.html
   https://github.com/zhangguanghuib/GH_PPKnowledgeSharing/blob/main/Copilot%20Studio/14%20Embedded%20Copilot%20Agent%20into%20website%20page/Code/testManualAuthenticationSSOBot.html
   https://github.com/zhangguanghuib/GH_PPKnowledgeSharing/blob/main/Copilot%20Studio/14%20Embedded%20Copilot%20Agent%20into%20website%20page/Code/testNoAuthenticationPublicBot.html
2. For the Manual Configuration, please check this article:
   https://github.com/zhangguanghuib/GH_PPKnowledgeSharing/tree/main/Copilot%20Studio/09%20Copilot%20Studio%20SSO
3. If the Copilot Agent is public, i.e no authentication, its knowledge source should be public access like public website, otherwise you will get error during conversation with Copilot Agent.


