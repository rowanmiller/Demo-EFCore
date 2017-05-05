# Demo: EF.Functions

## Everytime Setup

Required before/after each run of the demo
* Reset the starting point source code (`git reset --hard`)


## Demo Steps

* Show `BlogService` - point out the raw SQL query
* Show the unit test in `Tests` then run and show that it fails
* Update `BlogService` to use `EF.Functions.Like(...)`

```c#
return _db.Blogs.Where(b =>EF.Functions.Like(b.Url, likeExpression));
```

* Re-run the test and show it passes
* Show `Program.Main()`
* Run the app and show the generated SQL
 * I typically use SQL Profiler which is included in (SQL Server Management Studio)[https://www.microsoft.com/en-us/sql-server/sql-server-downloads] (free)