# Demo: Compiled Queries

## One Time Setup

Required one time to setup your machine
* Add the [AdventureWorks2014 sample database](https://msftdbprodsamples.codeplex.com/releases/view/125550) to you LocalDb instance (`(localdb)\mssqllocaldb`)


## Everytime Setup

Required before/after each run of the demo
* Reset the starting point source code (`git reset --hard`)


## Demo Steps

* Set `CompiledQueries` as the startup project.
* Implement the compiled query test as follows

```c#
var query = EF.CompileQuery((AdventureWorksContext db, string id) =>
    db.Customers.Single(c => c.AccountNumber == id));

using (var db = new AdventureWorksContext())
{
    foreach (var id in accountNumbers)
    {
        var customer = query(db, id);
    }
}
```

* Run and show results