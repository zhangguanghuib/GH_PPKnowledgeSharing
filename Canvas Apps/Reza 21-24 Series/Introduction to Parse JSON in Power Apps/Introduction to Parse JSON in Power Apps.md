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
3. Parse object JSON Array<br/>
```
[
  {
    "name": "Tom",
    "email": "tom@tech.com",
    "age": 23
  },
  {
    "name": "John",
    "email": "john@gmail.com",
    "age": 28
  },
  {
    "name": "Meghan",
    "email": "meghan@msn.com",
    "age": 33
  },
  {
    "name": "Alice",
    "email": "alice100@yahoo.com",
    "age": 41
  }
]
```
```
Text(Index(Table(ParseJSON(Trim((TextInputCanvas4.Value)))),2).Value.email)
```
<img width="1187" height="612" alt="image" src="https://github.com/user-attachments/assets/54b69610-811f-4a3b-8b84-a1c985daeee2" /><br/>

4. Parse JSON Object Array and bind to the gallery data source<br/>
```
[
  {
    "name": "Tom",
    "email": "tom@tech.com",
    "age": 23
  },
  {
    "name": "John",
    "email": "john@gmail.com",
    "age": 28
  },
  {
    "name": "Meghan",
    "email": "meghan@msn.com",
    "age": 33
  },
  {
    "name": "Alice",
    "email": "alice100@yahoo.com",
    "age": 41
  }
]
```
```
Table(ParseJSON(TextInputCanvas5.Value))
```
<img width="1411" height="1048" alt="image" src="https://github.com/user-attachments/assets/63fba523-58cb-4cbe-b857-410c581ba702" />><br/>

For each individual gallery fields<Br/>
```
Text(ThisItem.Value.name)
Text(ThisItem.Value.email)
Value(ThisItem.Value.age)
```
<img width="1855" height="1069" alt="image" src="https://github.com/user-attachments/assets/e3ca95e4-c430-4b7f-a8da-04eb1632ef21" /><br/>

5.Parse complex JSON Object <br/>
```
{
  "employees": [
    {
      "name": "Tom",
      "email": "tom@tech.com",
      "age": 23,
      "active": true
    },
    {
      "name": "John",
      "email": "john@gmail.com",
      "age": 28,
      "active": false
    },
    {
      "name": "Meghan",
      "email": "meghan@msn.com",
      "age": 33,
      "active": false
    },
    {
      "name": "Alice",
      "email": "alice100@yahoo.com",
      "age": 41,
      "active": false
    }
  ]
}
```
Gallery data source is set as <br/>
```
Filter(Table(ParseJSON(TextInputCanvas6.Value).employees), Boolean(ThisRecord.Value.active) =false)
```
All individual field:<br/>
```
Value(ThisItem.Value.age)
Text(ThisItem.Value.email)
Text(ThisItem.Value.name)
```
<img width="1767" height="991" alt="image" src="https://github.com/user-attachments/assets/4884f25a-a767-4e3f-8559-8042d9a40f80" /><br/>

6. Similar as above:<br/>
  <img width="1927" height="1029" alt="image" src="https://github.com/user-attachments/assets/47c4996c-5d51-494a-95a9-9a7c8fb462b9" /><br/>
<img width="1558" height="876" alt="image" src="https://github.com/user-attachments/assets/feb6fe8b-92c9-4dea-9386-739148837408" />


