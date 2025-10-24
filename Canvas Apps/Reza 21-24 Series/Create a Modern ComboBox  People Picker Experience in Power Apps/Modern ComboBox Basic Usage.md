# Modern ComboBox Basic Usage
Step 1:  Create a Sharepoint list as below <br/>
<img width="1988" height="754" alt="image" src="https://github.com/user-attachments/assets/5764b97b-23a5-4c65-a4a2-85e4b33369fe" /><br/>
Step 2:  Add a modern ComboBox control to the screen<br/>
<img width="345" height="306" alt="image" src="https://github.com/user-attachments/assets/6fa29ae5-575c-4b67-b6d1-a40384113b77" /><br/>
Adding datasource to this ComboBox Control<br/>
<img width="1352" height="508" alt="image" src="https://github.com/user-attachments/assets/c1ca57e7-baa2-415a-b0d7-4ebad42c65ec" /><br/>
<img width="2453" height="876" alt="image" src="https://github.com/user-attachments/assets/fa613cf1-d710-4744-ad87-e3559dc061f0" /><br/>
<img width="2046" height="1099" alt="image" src="https://github.com/user-attachments/assets/25c21d27-2070-4e54-a9d5-aab416547e43" />
Step 3: Run this screen <br/> 
<img width="655" height="435" alt="image" src="https://github.com/user-attachments/assets/dbaf2306-b532-48e3-baf5-08ff6e327782" /><br/> 

The code of this ComboBox Control is: <br/>
```
- ComboboxCanvas5:
    Control: ComboBox@0.0.51
    Properties:
      Items: =Filter('Work tracker01', StartsWith(Title, Self.SearchText))
      TriggerOutput: ='ComboboxCanvas.TriggerOutput'.Delayed
      X: =40
      Y: =40
    Children:
      - Title3:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Title"
            FieldName: ="Title"
            FieldType: ="s"
            Order: =1
      - Description2:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Description"
            FieldName: ="Description"
            FieldType: ="s"
            Order: =2
      - Category1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Category"
            FieldName: ="Category0"
            FieldType: ="E"
            Order: =3
      - Progress1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Progress"
            FieldName: ="Progress"
            FieldType: ="E"
            Order: =4
      - Priority1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Priority"
            FieldName: ="Priority"
            FieldType: ="E"
            Order: =5
      - Start date1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Start date"
            FieldName: ="StartDate"
            FieldType: ="D"
            Order: =6
      - Due date1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Due date"
            FieldName: ="DueDate"
            FieldType: ="D"
            Order: =7
      - Assigned to1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Assigned to"
            FieldName: ="AssignedTo0"
            FieldType: ="E"
            Order: =8
      - Notes1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Notes"
            FieldName: ="Notes"
            FieldType: ="s"
            Order: =9
      - Key stakeholders1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Key stakeholders"
            FieldName: ="KeyStakeholders"
            FieldType: ="E"
            Order: =10
      - Attachments1:
          Control: ComboBoxDataField@1.5.0
          Variant: textualColumn
          IsLocked: true
          Properties:
            FieldDisplayName: ="Attachments"
            FieldName: ="{Attachments}"
            FieldType: ="r*"
            Order: =11

```
