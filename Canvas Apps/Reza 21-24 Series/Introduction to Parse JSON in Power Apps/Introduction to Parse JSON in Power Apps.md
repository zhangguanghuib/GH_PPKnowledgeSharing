# Introduction to Parse JSON in Power Apps

1. Parse simple JSON
```json
{
  "name": "Tom",
  "email": "tom@tech.com",
  "age": 23,
  "active": true
}
```
<img width="1203" height="668" alt="image" src="https://github.com/user-attachments/assets/82f5e7e5-4657-48d0-9d4a-37c6df393db1" /><br/>
```
ParseJSON(Trim(txtInput.Value)).name
ParseJSON(Trim(txtInput.Value)).email
ParseJSON(Trim(txtInput.Value)).age
ParseJSON(Trim(txtInput.Value)).active
```
2. Parse simple JSON Array<br/>
```
["Ford", "BMW", "Fiat"]
```
<img width="1046" height="571" alt="image" src="https://github.com/user-attachments/assets/45a330e3-27f1-46c0-90ef-09c1b714b683" /><br/>
Please array parse is 1-based:
```
Text(Index(Table((ParseJSON(Trim(TextInputCanvas2.Value)))),1).Value)
Text(Index(Table((ParseJSON(Trim(TextInputCanvas2.Value)))),2).Value)
Text(Index(Table((ParseJSON(Trim(TextInputCanvas2.Value)))),3).Value)
```

