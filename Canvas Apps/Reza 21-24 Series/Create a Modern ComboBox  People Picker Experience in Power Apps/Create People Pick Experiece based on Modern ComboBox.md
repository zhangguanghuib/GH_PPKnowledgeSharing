# Create a Modern ComboBox People Picker Experience in Power Apps
Step 0:  Youtybe link: https://www.youtube.com/watch?v=m0HSaoDhYdg<br/>
Step 1: Create a SharePoint List as below:<br/>
<img width="2005" height="754" alt="image" src="https://github.com/user-attachments/assets/f35b71ba-6186-4f6b-a070-677a5a1a7099" /><br/>

Step 2:
Add a vertical gallery <br/>
<img width="2457" height="1048" alt="image" src="https://github.com/user-attachments/assets/b84a19f1-4940-4cd4-bbc5-38999951a4a9" /><br/>

Step 3:<br/>
Adding a modern form control <br/>
<img width="368" height="593" alt="image" src="https://github.com/user-attachments/assets/10fc54a2-eecc-4b96-9118-db059ea30a12" /><br/>

Step 4: <br/>
Unlock the data card for changing it: <br/>
<img width="1228" height="656" alt="image" src="https://github.com/user-attachments/assets/27a1b796-9850-4c83-a2c9-3c2b03fe22ae" />

For the ComboBox, set its Items<br/>
```
Office365Users.SearchUserV2({searchTerm:Self.SearchText, isSearchTermRequired:false, top:999}).value
```
<img width="1816" height="991" alt="image" src="https://github.com/user-attachments/assets/6747a71e-de2f-4451-bd0d-52a254206979" /><br/>

Set its DefaultSelectedItems: <br/>
```
Office365Users.SearchUserV2({searchTerm: ThisItem.'Assigned to'.Email, top:1}).value
```
<img width="1794" height="983" alt="image" src="https://github.com/user-attachments/assets/b2188889-0df9-424a-bd6e-5ec8f9499310" />
Set its InputTextPlaceholder <br/>

```
🔍Search for user
```

<img width="2062" height="1095" alt="image" src="https://github.com/user-attachments/assets/c801e64d-6e64-4db6-b6a6-b16d999eb71d" />

Step 5, for the DataCard, set its update property:<br/>

```
If(
    DataCardValue41.Selected.Mail = Blank(),
    Blank(),
    {
        Claims: Concatenate(
            "i:0#.f|membership|",
            DataCardValue41.Selected.Mail// Person email
        ),
        Department: "",
        DisplayName: DataCardValue41.Selected.DisplayName,
        Email: DataCardValue41.Selected.Mail,
        // Person email
        JobTitle: "",
        Picture: ""
    }
)
```
<img width="1775" height="1026" alt="image" src="https://github.com/user-attachments/assets/5c5d4fa4-96d4-4ab4-823c-20e4c3dc56e4" />

Step 6: set  the user profile image <br/>
Insert image control<br/>
<img width="1021" height="773" alt="image" src="https://github.com/user-attachments/assets/eab85cfd-e740-4c9b-bcff-1797d24b2e38" /><br/>
Set its Image Property:<br/>

```
If(!IsBlank(DataCardValue41.Selected.Id), Office365Users.UserPhotoV2(DataCardValue41.Selected.Id))
```
<img width="1783" height="880" alt="image" src="https://github.com/user-attachments/assets/743ab032-92ad-48f2-a7d0-178e7f5fde9b" />

Another steps need notice is, select the form, find the Fields and click add field and choose Display Name:

<img width="3288" height="1527" alt="image" src="https://github.com/user-attachments/assets/d0c0f6cc-2053-4be3-bd13-0723f232955d" />


Step 7: Test it <br/>
<img width="1921" height="1080" alt="image" src="https://github.com/user-attachments/assets/18bb229b-cebc-48e6-92db-4f9a6b6d4fe6" /><br/>
<img width="1957" height="1038" alt="image" src="https://github.com/user-attachments/assets/d07a6f38-bbe6-4b10-bf19-dba74fa923d1" />
<br/>

Step 8, the code of DataCard is<br/>
```
- Assigned to_DataCard1:
    Control: TypedDataCard@1.0.7
    Variant: ComboBoxEdit
    Properties:
      DataField: ="AssignedTo0"
      Default: =ThisItem.'Assigned to'
      DisplayName: =DataSourceInfo([@'Work tracker01'],DataSourceInfo.DisplayName,'AssignedTo0')
      Required: =false
      Update: |-
        =If(
            DataCardValue41.Selected.Mail = Blank(),
            Blank(),
            {
                Claims: Concatenate(
                    "i:0#.f|membership|",
                    DataCardValue41.Selected.Mail// Person email
                ),
                Department: "",
                DisplayName: DataCardValue41.Selected.DisplayName,
                Email: DataCardValue41.Selected.Mail,
                // Person email
                JobTitle: "",
                Picture: ""
            }
        )
      Width: =417
      X: =0
      Y: =3
    Children:
      - DataCardKey42:
          Control: Text@0.0.51
          MetadataKey: FieldName
          Properties:
            Height: =22
            Text: =Parent.DisplayName
            Weight: ='TextCanvas.Weight'.Semibold
            Width: =Parent.Width - 48
            Wrap: =false
            X: =24
            Y: =10
      - DataCardValue41:
          Control: ComboBox@0.0.51
          MetadataKey: FieldValue
          Properties:
            AccessibleLabel: =Parent.DisplayName
            DefaultSelectedItems: |-
              =Office365Users.SearchUserV2({searchTerm: ThisItem.'Assigned to'.Email, top:1}).value
            DisplayMode: =Parent.DisplayMode
            Height: =34
            InputTextPlaceholder: |-
              ="🔍Search for user"
            Items: =Office365Users.SearchUserV2({searchTerm:Self.SearchText, isSearchTermRequired:false, top:999}).value
            Required: =Parent.Required
            ValidationState: =If(IsBlank(Parent.Error), "None", "Error")
            Width: =311
            X: =92
            Y: =DataCardKey42.Y + DataCardKey42.Height + 4
          Children:
            - DisplayName1:
                Control: ComboBoxDataField@1.5.0
                Variant: textualColumn
                IsLocked: true
                Properties:
                  FieldDisplayName: ="DisplayName"
                  FieldName: ="DisplayName"
                  FieldType: ="s"
                  Order: =1
      - ErrorMessage42:
          Control: Text@0.0.51
          MetadataKey: ErrorMessage
          Properties:
            Height: =30
            Text: =Parent.Error
            Visible: =And(!IsBlank(Parent.Error), Parent.DisplayMode=DisplayMode.Edit)
            Width: =Parent.Width - 48
            X: =24
            Y: =DataCardValue41.Y + DataCardValue41.Height
      - StarVisible42:
          Control: Text@0.0.51
          MetadataKey: FieldRequired
          Properties:
            Align: ='TextCanvas.Align'.Center
            Height: =20
            Text: ="*"
            Visible: =And(Parent.Required, Parent.DisplayMode=DisplayMode.Edit)
            Width: =30
            Y: =DataCardKey42.Y
      - Image2:
          Control: Image@2.2.3
          Properties:
            Height: =34
            Image: =If(!IsBlank(DataCardValue41.Selected.Id), Office365Users.UserPhotoV2(DataCardValue41.Selected.Id))
            Width: =57
            X: =30
            Y: =36
```
