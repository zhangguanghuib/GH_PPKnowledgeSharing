# Send Email from Copilot Agent With Adaptive Card
1. Create a new topic as below: <br/>
   <img width="2401" height="1259" alt="image" src="https://github.com/user-attachments/assets/411aa8c1-d901-4fdd-a6c4-72f840212c3f" /><br/><hr/>
2. Add an Adaptive Card as below <br/>
   <img width="834" height="922" alt="image" src="https://github.com/user-attachments/assets/d48d2216-90e5-420b-8e22-db14eaac288d" /><br/><hr/>
   ```
     {
      "type": "AdaptiveCard",
      "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
      "version": "1.5",
      "body": [
          {
              "type": "TextBlock",
              "text": "IT Issue Submission",
              "weight": "Bolder",
              "size": "Large",
              "wrap": true
          },
          {
              "type": "Input.Text",
              "id": "Title",
              "placeholder": "Enter the issue title",
              "label": "Title",
              "maxLength": 100
          },
          {
              "type": "Input.Text",
              "id": "IssueDescription",
              "placeholder": "Enter the issue details",
              "label": "Issue Description",
              "maxLength": 500,
              "isMultiline": true
          }
      ],
      "actions": [
          {
              "type": "Action.Submit",
              "title": "Submit"
          }
      ]
  }
  
   ```
