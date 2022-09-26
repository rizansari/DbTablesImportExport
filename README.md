# DbImportExport library for .NET Core and .NET 5

Library Version: v1.0.3


## Installation

```powershell
Install-Package DbImportExport
```

## Usage

### Export

```
    var config = new ExportConfig()
    {
        ConnectionString = "CONNECTION_STRING_HERE",
        DatabaseProvider = "System.Data.SqlClient",
        Destination = @"D:\temp\db\backup.bak",
        TableNames = (new string[] { "TABLE1" }).ToList()
    };
    var export = new Export(config);
    export.ExportToFile();
```

### Import

```
    var config = new ImportConfig()
    {
        ConnectionString = "CONNECTION_STRING_HERE",
        DatabaseProvider = "System.Data.SqlClient",
        Source = @"D:\temp\db\backup.bak",
        IdentityInsert = true,
        Overwrite = true
    };
    var import = new Import(config);
    import.ImportFromFile();
```