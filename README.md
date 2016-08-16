# Ado-Net-Helper
SQL Sorgularını çalıştırabileceğiniz ve sonuçlarını listeleyebileceğiniz bir yardımcı ADO.NET kütüphanesidir.

[Sample Project(örnek proje)](https://github.com/kadirmuratbaseren/Using-Ado-Net-Helper)

## Code-Samples

### Creating Database Object
```c#
// Create instance for Database object.
DB = new Database(ConnectionString);
```

### How to use RunQuery method
```c#
// RunQuery method runs queries. Return affected row count value.

int result =
    DB.RunQuery("INSERT INTO Books(Name, Author, Description, Price) VALUES(@Name, @Author, @Desc, @Price)",
        new ParamItem() { ParamName = "@Name", ParamValue = "Jungle Book" },
        new ParamItem() { ParamName = "@Author", ParamValue = "K. Murat Başeren" },
        new ParamItem() { ParamName = "@Desc", ParamValue = "About book subject" },
        new ParamItem() { ParamName = "@Price", ParamValue = 25 });

// Shows affected row count in output window.
Debug.WriteLine("Affected row(s) count (RunQuery - Insert) : " + result);
```

### How to use GetTable method
```c#
// Get Datatable with sql datas
// I use generic ParamItem<T>

DataTable dt =
    DB.GetTable("SELECT Id, Name, Author, Description, Price FROM Books WHERE Price > @PriceVal",
        new ParamItem() { ParamName = "@PriceVal", ParamValue = 25m });

// Shows rows count in output window.
Debug.WriteLine("Result table row count(RunQuery - GetTable) : " + dt.Rows.Count);
```

### How to use RunProc method
```c#
// Add MyStoredProc1 and MyStoredProc2 sample procedures..

/* Sample Procedures DDLs
 
    CREATE PROCEDURE MyStoredProc1
      @MinPrice decimal(18,2),
      @MaxPrice decimal(18,2)
    AS
    BEGIN
      SELECT [Id], [Name], [Author], [Description], [Price] 
      FROM Books
      WHERE Price > @MinPrice AND Price < @MaxPrice
    END
    GO

    CREATE PROCEDURE MyStoredProc2
      @NewPrice decimal(18,2)
    AS
    BEGIN
      UPDATE Books SET Price = @NewPrice
      WHERE Author LIKE '%Charles%'
    END
    GO

 */

// After...

// Get Table..
DataTable dt = DB.RunProc("MyStoredProc1",
    new ParamItem() { ParamName = "@MinPrice", ParamValue = 20m },
    new ParamItem() { ParamName = "@MaxPrice", ParamValue = 25m });

// Shows rows count in output window.
Debug.WriteLine("Result table row count(RunProc - MyStoredProc1) : " + dt.Rows.Count);


// No return value..
DB.RunProc("MyStoredProc2",
    new ParamItem() { ParamName = "@NewPrice", ParamValue = 29m });
```
