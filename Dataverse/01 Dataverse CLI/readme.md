`dataverse auth create --environment https://myorg.crm.dynamics.com`


dataverse data get --table accounts --id 00000000-0000-0000-0000-000000000001
dataverse data get --target erp --table Currencies --key "CurrencyCode='AED'" --select "CurrencyCode,Name"

dataverse data query --table accounts --select "name,revenue" --filter "revenue gt 1000000"
dataverse data query --fetchxml "<fetch></fetch><entity name='account'></fetch>"
dataverse data query --sql "SELECT name, revenue FROM account WHERE revenue > 1000000"
dataverse data query --target erp --table Currencies --select "CurrencyCode,Name" --top 10

dataverse mcp https://myorg.crm.dynamics.com --validate
