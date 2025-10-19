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
