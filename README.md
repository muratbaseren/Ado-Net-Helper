# Ado-Net-Helper
SQL Sorgularını çalıştırabileceğiniz ve sonuçlarını listeleyebileceğiniz bir yardımcı ADO.NET kütüphanesidir.

[Sample Project(örnek proje)](https://github.com/kadirmuratbaseren/Using-Ado-Net-Helper)

| Build server| Name             | Framework           | Status      |
|-------------|------------------|---------------------|-------------|
| AppVeyor    | AdoNetHelper     | NET Framework 4.8.1 |[![Build status](https://ci.appveyor.com/api/projects/status/aw4iajsf45eyl6g0/branch/master?svg=true)](https://ci.appveyor.com/project/muratbaseren/ado-net-helper/branch/master) |
| AppVeyor    | AdoNetHelperCore | NET 9               |[![Build status](https://ci.appveyor.com/api/projects/status/59cnbwtkm8i9aw30/branch/master?svg=true)](https://ci.appveyor.com/project/muratbaseren/ado-net-helper-core/branch/master)
 |

## Code-Samples

### Creating Database Object
```c#
// Create instance for Database object.
Database DB = new Database(ConnectionString);
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

### How to use RunScalar method
```c#
// Execute scalar query and convert result
int bookCount = DB.RunScalar<int>("SELECT COUNT(*) FROM Books");

// Execute scalar query async and convert result
int bookCount = await DB.RunScalarAsync<int>("SELECT COUNT(*) FROM Books");
```

### How to use RunProc method

#### Create sample stored procedures

```sql
-- Add MyStoredProc1 and MyStoredProc2 sample procedures..
-- Sample Procedures DDLs
 
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
    
```

#### Using RunProc method
```c#
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

### Async usage
```c#
// Example of using async methods
int affectedRows = await DB.RunQueryAsync(
    "INSERT INTO Books(Name, Author, Description, Price) VALUES(@Name, @Author, @Desc, @Price)",
    new ParamItem() { ParamName = "@Name", ParamValue = "Jungle Book" },
    new ParamItem() { ParamName = "@Author", ParamValue = "K. Murat Başeren" },
    new ParamItem() { ParamName = "@Desc", ParamValue = "About book subject" },
    new ParamItem() { ParamName = "@Price", ParamValue = 25 });

DataTable asyncTable = await DB.GetTableAsync(
    "SELECT Id, Name, Author, Description, Price FROM Books WHERE Price > @PriceVal",
    new ParamItem() { ParamName = "@PriceVal", ParamValue = 25m });
```

### RunFunction usage
```c#
// Example of calling a SQL function that returns table
DataTable fnTable = DB.RunFunction("dbo.GetBooksByPrice",
    new ParamItem() { ParamName = "@MinPrice", ParamValue = 10m },
    new ParamItem() { ParamName = "@MaxPrice", ParamValue = 20m });

// Async version
DataTable asyncFnTable = await DB.RunFunctionAsync("dbo.GetBooksByPrice",
    new ParamItem() { ParamName = "@MinPrice", ParamValue = 10m },
    new ParamItem() { ParamName = "@MaxPrice", ParamValue = 20m });
```

### Export sınıfı kullanımı
```c#
// DataTable verisini PDF dosyasına dönüştürme
Export exporter = new Export();
byte[] pdfBytes = await exporter.ToPdfAsync(dt);
File.WriteAllBytes(@"C:\\temp\\table.pdf", pdfBytes);

// DataTable verisini HTML dosyasına dönüştürme
byte[] htmlBytes = await exporter.ToHtmlAsync(dt);
File.WriteAllBytes(@"C:\\temp\\table.html", htmlBytes);

// DataTable verisini CSV dosyasına dönüştürme
byte[] csvBytes = await exporter.ToCsvAsync(dt);
File.WriteAllBytes(@"C:\\temp\\table.csv", csvBytes);
```

### Backup ve Restore metodları
```c#
// Veritabanı yedeği oluşturma
DB.Backup("BookDb", @"C:\\temp\\BookDb.bak");

// Yedekten veritabanını geri yükleme
DB.Restore("BookDb", @"C:\\temp\\BookDb.bak");

// Async kullanımı
await DB.BackupAsync("BookDb", @"C:\\temp\\BookDb.bak");
await DB.RestoreAsync("BookDb", @"C:\\temp\\BookDb.bak");
```

### Tablo klonlama metodları
```c#
// Yapı kopyası oluşturma (veri olmadan)
await DB.CloneTableStructureAsync("Books", "BooksCopy");

// Yapı ve verilerle beraber kopya oluşturma
await DB.CloneTableWithDataAsync("Books", "BooksCopyWithData");
```

### Transaction kullanımı (Henüz Test Edilmedi !!)
```c#
// Transaction başlatma
await DB.BeginTransactionAsync();
try
{
    DB.RunQuery("UPDATE Books SET Price = Price + 1");
    DB.RunQuery("INSERT INTO Logs(Message) VALUES(@msg)",
        new ParamItem() { ParamName = "@msg", ParamValue = "Prices updated" });
    DB.Commit();
}
catch
{
    DB.Rollback();
    throw;
}
```
