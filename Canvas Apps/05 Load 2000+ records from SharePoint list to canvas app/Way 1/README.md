# https://www.youtube.com/watch?v=YQQaLVkhPGo\&t=3s<br/>

<img width="1615" height="749" alt="image" src="https://github.com/user-attachments/assets/250b0f01-8522-41e6-818f-f3e041fe6474" /><br/>

<img width="912" height="805" alt="image" src="https://github.com/user-attachments/assets/b7fd435e-11e5-42b9-ad93-67d4cecbd2fa" /><br/>

<img width="423" height="654" alt="image" src="https://github.com/user-attachments/assets/0ffab4b9-214f-4bfd-bd84-a763ecf5f1de" /><br/>


Create a SharePoint List from Excel<br/>

<img width="876" height="599" alt="image" src="https://github.com/user-attachments/assets/fcc9925e-9aa9-4f66-87b3-132b8695d564" /><br/>

Choose the Excel Table <br/>
<img width="1191" height="812" alt="image" src="https://github.com/user-attachments/assets/bf808081-bf79-4d1a-bc57-fdb08b2ece72" /><br/>

Show the "ID" Column<br/>
<img width="1821" height="1104" alt="image" src="https://github.com/user-attachments/assets/f6043cc6-ecb5-4ddf-a1d2-88cdde06dd87" /><br/>
<img width="1770" height="592" alt="image" src="https://github.com/user-attachments/assets/ec2b0a59-b1bb-4e09-a802-c30967655fd1" /><br/>

Create a blank screen, add a gallery:<br/>

<img width="1395" height="729" alt="image" src="https://github.com/user-attachments/assets/6c2f3567-3a93-4a1d-9931-ee4507e5ab1f" /><br/>

OnVisible of this Screen<br/>
<img width="1374" height="683" alt="image" src="https://github.com/user-attachments/assets/96d10d5d-7023-4e18-8a87-3e03ab83246e" /><br/>

Set the Gallery Items = colBook<br/>
<img width="1369" height="604" alt="image" src="https://github.com/user-attachments/assets/45f3c25e-e24f-4331-af35-2b05199aaa51" /><br/>
See now there are 2000 records in the gallery<br/>
<img width="1526" height="707" alt="image" src="https://github.com/user-attachments/assets/ca9bf426-2725-4511-b8e2-ef3cc50d8095" /><br/>

<hr/>
Create IndexId Column<br/>
<img width="1752" height="532" alt="image" src="https://github.com/user-attachments/assets/e524876f-c9da-41c7-9077-974f68ad63a6" /><br/>
<img width="1273" height="726" alt="image" src="https://github.com/user-attachments/assets/b537a5b6-4f12-403e-8ac6-d5770c567d3c" /><br/>
Already created IndexId Column<br/>
<img width="942" height="542" alt="image" src="https://github.com/user-attachments/assets/fad32f3a-c1f8-45cc-b39e-a2c1c1f1f8a3" /><br/>
Create an IndexId:<br/>
<img width="1285" height="718" alt="image" src="https://github.com/user-attachments/assets/6447e35a-4129-4ce4-8c52-e2996e9024ba" /><br/>

Using ID will give delegation warning,  but IndexId will not <br/>
<img width="1589" height="536" alt="image" src="https://github.com/user-attachments/assets/1452cdb5-f416-483a-acea-a28e898ada38" /><br/>

Now see 4000 rows got loaded<br/>
<img width="1013" height="498" alt="image" src="https://github.com/user-attachments/assets/89f5886e-4f8a-454e-bdc5-6f7cb93deda3" /><br/>

<img width="1465" height="549" alt="image" src="https://github.com/user-attachments/assets/48465803-1a09-4f12-b733-03db6f3c5e70" /><br/>
<img width="1430" height="476" alt="image" src="https://github.com/user-attachments/assets/09bd1d34-e60b-40a8-91bc-34a6c18f8672" /><br/>
<img width="1403" height="522" alt="image" src="https://github.com/user-attachments/assets/44e0ef50-fa5b-40ec-be73-0973c4b21357" /><br/>
<img width="1418" height="737" alt="image" src="https://github.com/user-attachments/assets/cbf40bae-9950-4945-8bfc-2440ec073935" /><br/>

Get the iterations<br/>
<img width="1115" height="476" alt="image" src="https://github.com/user-attachments/assets/9b244844-a335-4165-b2fe-ca2184aaa170" /><br/>

Update this code<br/>
<img width="1446" height="456" alt="image" src="https://github.com/user-attachments/assets/1648dc04-2b5e-4839-9919-194330ea43db" /><br/>

<img width="1738" height="525" alt="image" src="https://github.com/user-attachments/assets/1de688dc-07be-447f-9d05-f02316f8efe1" /><br/>

<img width="1409" height="687" alt="image" src="https://github.com/user-attachments/assets/1a6dfc42-811e-45b2-a812-aa43c39a2129" /><br/>

```
Clear(colBook);
ForAll(Sequence(Round(First(Sort(Book1,Index_ID,SortOrder.Descending)).Index_ID / 2000, 0),1,1),
    With({_firstID: (ThisRecord.Value-1)*2000, _lastID: ThisRecord.Value * 2000},
        Collect(colBook,Filter(Book1,Index_ID> _firstID &&Index_ID<= _lastID))
    )
)
```

