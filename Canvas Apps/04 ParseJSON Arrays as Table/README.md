# Parse JSON Array to Table

1. The most normal way：<br/>
```
Set(
    varJson,
    ReadSharePointListJsonString.Run().result
);
ClearCollect(
    colData,
    ForAll(
        ParseJSON(varJson),
        {
            Title: Text(ThisRecord.Title),
            AssetType: Text(ThisRecord.AssetType),
            RepairShop: Text(ThisRecord.RepairShop)
        }
    )
);
Notify("Read Json from flow done", NotificationType.Success);
```

2. Create untyped table or collection, but it can be used as gallery data source or get any column/row value<br/>
```
UpdateContext({inputText: ReadSharePointListJsonString.Run().result}); 

ClearCollect(parsedTable, Table(ParseJSON(inputText)));
UpdateContext({firstRecord: Index(parsedTable, 1).Value});

UpdateContext({firstRecord01: Table(ParseJSON(inputText))});

UpdateContext({columns: ColumnNames(firstRecord.Value)});

Notify("Done", NotificationType.Success);

```
<img width="1714" height="1038" alt="image" src="https://github.com/user-attachments/assets/af6dd3bb-c409-486c-a41a-b6849a039d70" /><br/>
<img width="1502" height="941" alt="image" src="https://github.com/user-attachments/assets/9a4e24f1-9f31-47e1-b3ff-48d60d07f498" /><br/>
<img width="1602" height="994" alt="image" src="https://github.com/user-attachments/assets/ab44a9ce-ec15-477e-a408-9415c22d709b" /><br/>

Get any row/column value <br/>
<img width="1833" height="528" alt="image" src="https://github.com/user-attachments/assets/929b130d-af70-44f3-8eb9-b7c60aa118bc" />



