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

3. Flaten to make one column one row <br/>
 There are two versions<br/>
 ```
UpdateContext({ inputText: ReadSharePointListJsonString.Run().result });

ClearCollect(rawTable, Table(ParseJSON(inputText)));
UpdateContext({ columns: ColumnNames(First(rawTable).Value) });

Clear(normalizedTable);

ForAll(
    Sequence(CountRows(rawTable)) As seq,
    With(
        {
            rowObj: Index(rawTable, seq.Value).Value
            // If Index() is not available in your tenant, use:
            // rowObj: Last(FirstN(rawTable, seq.Value)).Value
        },
        Collect(
            normalizedTable,
            ForAll(
                columns As col,
                {
                    RowIndex: seq.Value,
                    ColumnName: Text(col.Value),
                    ColumnValue: Text(Column(rowObj, Text(col.Value)))
                }
            )
        )
    )
);

Notify("Done", NotificationType.Success);
 ```
<img width="1940" height="1289" alt="image" src="https://github.com/user-attachments/assets/1e406ae2-b643-44b1-94e8-ebc15e1ae3fd" /><br/>

4.  One column one row version 2 <br/>
```
UpdateContext({ inputText: ReadSharePointListJsonString.Run().result });

ClearCollect(rawTable, Table(ParseJSON(inputText)));

// optional: force schema/types for the target collection
ClearCollect(normalizedTable1, { RowIndex: 0, ColumnName: "", ColumnValue: "" });
Remove(normalizedTable1, First(normalizedTable1));

ForAll(
    Sequence(CountRows(rawTable)) As seq,
    With(
        {
            rowObj: Index(rawTable, seq.Value).Value
            // if Index() is unavailable in your tenant:
            // rowObj: Last(FirstN(rawTable, seq.Value)).Value
        },
        ForAll(
            ColumnNames(rowObj) As col,
            Patch(
                normalizedTable1,
                Defaults(normalizedTable1),
                {
                    RowIndex: Value(seq.Value),
                    ColumnName: Text(col.Value),
                    ColumnValue: Text(Column(rowObj, Text(col.Value)))
                }
            )
        )
    )
);

Notify("Done", NotificationType.Success);
```
The result is:<br/>
<img width="1645" height="1258" alt="image" src="https://github.com/user-attachments/assets/fb8c2ccc-1a64-4621-98b2-f019a077380a" /><br/>

5.  One row to one row,  but need nested table<br/>
```
UpdateContext({ inputText: ReadSharePointListJsonString.Run().result });

ClearCollect(rawTable, Table(ParseJSON(inputText)));
UpdateContext({ columns: ColumnNames(First(rawTable).Value) });

Clear(normalizedTable);

ForAll(
    Sequence(CountRows(rawTable)) As seq,
    With(
        {
            rowObj: Index(rawTable, seq.Value).Value
        },
        Collect(
            normalizedTable,
            {
                RowIndex: Value(seq.Value),   // primary key
                Fields: ForAll(
                    columns As col,
                    {
                        ColumnName: Text(col.Value),
                        ColumnValue: Text(Column(rowObj, Text(col.Value)))
                    }
                )
            }
        )
    )
);

Notify("Done", NotificationType.Success);
```
<img width="1571" height="864" alt="image" src="https://github.com/user-attachments/assets/705a9d30-49bc-4db2-b92d-1e8683bb49a2" /><br/>

<img width="1075" height="847" alt="image" src="https://github.com/user-attachments/assets/5bf3d27c-9cf3-49b5-8736-a2933951e395" /> <br/>
