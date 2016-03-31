The demo is broken up into a series of sections that showcase specific features. Each section has a corresponding slide in the slide deck, which you can use to provide a quick summary of the feature before demoing it. This also helps break up the long demo section of the talk into logical 5-10min chunks, which helps folks keep up to speed.

The demos are largely self contained, so you can pick and chose which ones you show, and change the order of them.

# One Time Setup

Required once on a machine to get it ready for these demos

* Install Visual Studio 2015
* Install SQL Server Management Studio
* Clone this repository to your local machine (or [download the source code zip](https://github.com/rowanmiller/Demo-EFCore/archive/master.zip) and extract it)
* Add the [AdventureWorks2014 sample database](https://msftdbprodsamples.codeplex.com/releases/view/125550) to you LocalDb instance (`(localdb)\mssqllocaldb`)
* **Optional:** If you want to show the performance impact of SaveChanges batching against a high-latency database, then create an AdventureWorks2014 database on an SQL Azure instance. Run the following script to create the required schema.

```
CREATE SCHEMA Production
GO

CREATE SCHEMA Sales
GO

CREATE TABLE [Production].[ProductCategory](
	[ProductCategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [dbo].[Name] NOT NULL,
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ProductCategory_rowguid]  DEFAULT (newid()),
	[ModifiedDate] [datetime] NOT NULL CONSTRAINT [DF_ProductCategory_ModifiedDate]  DEFAULT (getdate()),
	CONSTRAINT [PK_ProductCategory_ProductCategoryID] PRIMARY KEY CLUSTERED ([ProductCategoryID] ASC))
GO

CREATE TABLE [Sales].[Customer](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NULL,
	[StoreID] [int] NULL,
	[TerritoryID] [int] NULL,
	[AccountNumber] [nvarchar](200) NULL,
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ProductCategory_rowguid]  DEFAULT (newid()),
	[ModifiedDate] [datetime] NOT NULL CONSTRAINT [DF_ProductCategory_ModifiedDate]  DEFAULT (getdate()),
	CONSTRAINT [PK_Customer_CustomerID] PRIMARY KEY CLUSTERED ([CustomerID] ASC))
GO
```

# Everytime Setup

Required before/after each run thru the demos

* Reset the starting point source code
 * If you cloned the repo, you can use `git reset --hard` to revert changes
 * If you downloaded the zip, you should just keep a copy to replace the code you modified during the demo
* Delete NoteTaker local databases
 * UWP on local machine - open `%LOCALAPPDATA` and search for `Notes.db`, once you find the folder save a shortcut for the future
 * UWP on phone emulator - close the emulator, it's also best to restart it now too so you don't wait for it to bootup during the talk
 * WinForms - delete `Notes.db` from bin/debug folder of the `NoteTaker.Classic` project
* Delete `Demo.EFCore101` database from your LocalDb intsance
* Run the `FromSql` project so that you have the database and can show the TVF before running the app during the demo
* Reset the `IDENTITY` column on the table used for `INSERT` performance testing in the `AdventureWorks2014` database
 * You don't need to do this every time, but you may run out of identity values if you run the demo a lot of times without ever doing this

```
DECLARE @currentMax AS int = (SELECT MAX(ProductCategoryId) FROM [Production].[ProductCategory]);
DBCC CHECKIDENT ('[Production].[ProductCategory]', RESEED, @currentMax);
```

* **Optional:** If you want a clean "remote" database for NoteTaker, then either drop it or delete everything from the `Note` table (`DELETE FROM dbo.Note`)
* **Optional:** Delete `Demo.ReplacingServices` database to avoid waiting for it to be dropped during the demo

# Demo: EF Core 101

* Set `EFCore101` as the startup project.
* Open `Program.cs` and replace the TODO in `BloggingContext` with the following

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.EFCore101;Trusted_Connection=True;");
}
```

* Replace the TODO in `Program.Main(..)` with the following code

```
// Create the database
db.Database.EnsureCreated();

// Save some data
var blog = new Blog { Name = "The Dog Blog", Url = "http://sample.com/dogs" };
db.Blogs.Add(blog);
db.SaveChanges();

// Query some data
var blogs = db.Blogs.OrderBy(b => b.Name).ToList();
foreach (var item in blogs)
{
    Console.WriteLine(item.Name);
}
```

* Run (Ctrl+F5) and show output
* Open the database on your LocalDb instance and show the schema (I usually use SQL Server Management Studio)

# Demo: Performance

* Set `Performance` as the startup project.

## Part 1: Simple Query

* Add the following code to both test methods, then run (Ctrl+F5) and show output

```
db.Customers.ToList
```

## Part 2: More Complex Query

* Add the following code to both test methods, then run (Ctrl+F5) and show output

```
db.Customers
    .Where(c => !c.AccountNumber.EndsWith("1"))
    .OrderBy(c => c.AccountNumber)
    .ThenBy(c => c.ModifiedDate)
    .Skip(100)
    .GroupBy(c => c.TerritoryID)
    .ToList();
```

## Part 3: Adding & Saving

* Add the following code to both test methods (note you need to change the type that is created), then run (Ctrl+F5) and show output

```
using (var db = new EF6.AdventureWorksContext())
{
    for (int i = 0; i < 1000; i++)
    {
        db.ProductCategories.Add(new EF6.ProductCategory { Name = $"Test {Guid.NewGuid()}" });
    }
    db.SaveChanges();
}
```

## Part 4: Adding & Saving (Optimized EF6 Code)

* Change EF6 code to the following, then run (Ctrl+F5) and show output

```
using (var db = new EF6.AdventureWorksContext())
{
    db.Configuration.AutoDetectChangesEnabled = false;
    var categories = new EF6.ProductCategory[1000];
    for (int i = 0; i < 1000; i++)
    {
        categories[i] = new EF6.ProductCategory { Name = $"Test {Guid.NewGuid()}" };
    }
    db.ProductCategories.AddRange(categories);
    db.SaveChanges();
}
```


# Demo: Simplified Metadata API

* Set `Metadata` as the startup project.
* Show how complex `PrintEF6Mappings()` is
* Replace the TODO in `PrintEFCoreMappings()` with the following code

```
foreach (var entity in db.Model.GetEntityTypes())
{
    Console.WriteLine($" {entity.ClrType.Name} => {entity.SqlServer().TableName}");
}
```

* Run (Ctrl+F5) and show output


# Demo: Extensible Core

## Part 1: Consuming Services

* Set `ConsumingServices` as the startup project
* Update the `Main` method with the following code

```
var serviceProvider = db.GetInfrastructure<IServiceProvider>();
var typeMapper = serviceProvider.GetService<IRelationalTypeMapper>();

Console.WriteLine($"Type mapper in use: {typeMapper.GetType().Name}");
Console.WriteLine($"Mapping for bool: {typeMapper.GetMapping(typeof(bool)).DefaultTypeName}");
Console.WriteLine($"Mapping for string: {typeMapper.GetMapping(typeof(string)).DefaultTypeName}");
```

* Run (Ctrl+F5) and show output

## Part 2: Replacing Services

* Set `ReplacingServices` as the startup project
* Show `Blog.Metadata` with the custom `[Xml]` attribute
* Open `CustomSqlServerTypeMapper`
* Replace the TODO in `GetMapping(IProperty property)` with the following code

```
if (property.HasClrAttribute<XmlAttribute>())
{
    return _xml;
}
```

* Add a constructor to `BloggingContext` that passes in an external service provider

```
public BloggingContext(IServiceProvider serviceProvider)
    : base(serviceProvider)
{ }
```

* Replace the TODO in `Main` with the following code

```
var serviceCollection = new ServiceCollection();

serviceCollection
    .AddEntityFramework()
    .AddSqlServer();

serviceCollection.AddSingleton<SqlServerTypeMapper, CustomSqlServerTypeMapper>();

using (var db = new BloggingContext(serviceCollection.BuildServiceProvider()))
{
    ...
```

* Run (Ctrl+F5) and show output
* Open the database on your LocalDb instance and show the xml column in the table (I usually use SQL Server Management Studio)


# Demo: Same Model, Different Platforms

## Part 1: UWP

* Set `NoteTaker.Modern` as the startup project
* Replace the TODO in `NoteContext` (`NoteTaker` project) with the following code

```
optionsBuilder.UseSqlite("Filename=Notes.db");
```

* Open the code behind for NewNotePage.xaml
* Replace the TODO with the following code

```
using (var db = new NoteContext())
{
    db.Notes.Add(note);
    db.SaveChanges();
}
```

* Show the code behind `ExistingNotesPage.xaml`
* Run the app and show that notes are saved and queried
* **Optional:** open up the folder for the app and show the `Notes.db` file on disk
* **Optional:** swap to deploy to the phone emulator and show the app working there

## Part 2: WinForms

* Set `NoteTaker.Classic` as the startup project
* Open the code behing `MainForm.cs`
* Replace the TODO in `save_Click()` with the following code (or copy/paste it from the UWP app)

```
using (var db = new NoteContext())
{
    db.Notes.Add(note);
    db.SaveChanges();
}
```

* Run the app and show that notes are saved and queried
* **Optional:** open up the `bin\debug` folder and show the `Notes.db` file on disk


# Demo: Same Model, Different Databases

* Modify `NoteContext` (`NoteTaker` project) to take external options (new constructor and defensive code in `OnConfiguring`)

```
public class NoteContext : DbContext
{
    public NoteContext()
    { }

    public NoteContext(DbContextOptions options)
        : base(options)
    { }

    public DbSet<Note> Notes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Filename=Notes.db");
        }
    }
}
```

* **Optional:** Add the `IsUploaded` property to the `DataGridView` on `MainForm`
 * Open `MainForm` in the designer
 * Click on the existing notes grid view
 * Click the little arrow that shows in the top right of the grid view
 * Select `Edit Columns...`
 * Click `Add...`
 * Select `IsUploaded` in the property list
 * Check the `Read Only` check box
 * Click `OK`
 * Click `Close`
 * Change the `AutoSizeMode` for the new column to `AllCells`

* Open the code behing `MainForm.cs`
* Replace the TODO in `upload_Click()` with the following code

```
var connectionString = ConfigurationManager.ConnectionStrings["RemoteDatabase"].ConnectionString;
var builder = new DbContextOptionsBuilder();
builder.UseSqlServer(connectionString);
var options = builder.Options;

using (var db = new NoteContext(options))
{
    db.Database.EnsureCreated();
    db.Notes.AddRange(newNotes);
    db.SaveChanges();
}
```

* Run the app, add a few more notes, then select `File -> Upload`
* Open the "remote" database on your LocalDb instance and show the uploaded data (I usually use SQL Server Management Studio)

# Demo: SQL Generation Improvements

## Part 1: SaveChanges Batching

* Set `Batching` as the startup project
* Walk thru the code
* Start some form of profiling (I use SQL Server Profiler that comes with SQL Management Studio)
* Run the app
* Show that all changes were persisted in a single command

* In `BloggingContext.OnConfiguring(..)` append `.MaxBatchSize(5)` after the call to `UseSqlServer(..)`
* Run the app and show that two commands are sent to the database

* **Optional:** Use the performance demo to show how much difference batching makes against a high-latency database
 * Set `Performance` as the startup project
 * If you already did the Performance demo then the code should still be in the state from Part 4 of that demo
 * Change the connection string at the bottom of `App.config` to point to the Azure database (see the One Time Setup section for details)
 * Run the app (you should see EF Core be ~90% faster than EF6.x)

## Part 2: FromSql()

* Set `FromSql` as the startup project
* Open the `Demo.FromSql` database on your LocalDb instance and show the `dbo.SearchBlogs` TVF under `Programability -> Functions -> Table Valued Functions` (I usually use SQL Server Management Studio)
* Start some form of profiling (I use SQL Server Profiler that comes with SQL Management Studio)
* Open `Program.cs` and relace the TODO with the following code

```
var blogs = db.Blogs.FromSql("SELECT * FROM dbo.SearchBlogs(@p0)", term);
```

* Run the app and show that exactly the provided SQL is run
* Replace the query with the following code

```
var blogs = db.Blogs.FromSql("SELECT * FROM dbo.SearchBlogs(@p0)", term)
    .OrderBy(b => b.Url)
    .Select(b => b.Url);
```

* Run the app and show that the original SQL is composed upon
