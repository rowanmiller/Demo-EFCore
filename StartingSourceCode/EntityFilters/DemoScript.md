# Demo: Entity Filters

## Everytime Setup

Required before/after each run of the demo

* Reset the starting point source code (`git reset --hard`)


## Demo Steps

* Set `EntityFilters` as the startup project
* Show that we have `Post.SoftDelete` and `Blog.TenantId`
* Run and show that deleted posts, and blogs from other tenants, are being returned

### Part 1: Soft Delete

* Add the following line to `BloggingContext.OnModelCreating(...)`

```c#
modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
```

* Run and show that deleted posts are filtered out

### Part 2: Multi Tenant

* Add the following line to `BloggingContext.OnModelCreating(...)`

```c#
modelBuilder.Entity<Blog>().HasQueryFilter(b => b.TenantId == _tenantId);
```

* Talk about how `OnModelCreating(...)` is only run once for all contexts, but the filter is stored as an expression so that `_tenantId` is read lazily from the context that is executing the query
* Run and show that data from other tenants is filtered out

### Part 3: Flexible Mapping

This isn't specific to Entity Filters, but the multi tenant scenario is a good one to show off some of EF Core's mapping abilities

* Change `Blog.TenantId` to a private field

```c#
private string _tenantId;
```

*  Map it in `OnModelCreating(...)`, also make it readonly so it can't be changed

```c#
modelBuilder.Entity<Blog>().Property<string>("TenantId").HasField("_tenantId").Metadata.IsReadOnlyAfterSave = true;
```

* Update the query filter

```c#
modelBuilder.Entity<Blog>().HasQueryFilter(b => EF.Property<string>(b, "TenantId") == _tenantId);
```

* Update the `Main()` method to get values from the change tracker

```c#
Console.WriteLine($"{blog.Url.PadRight(33)} [Tenant: {db.Entry(blog).Property("TenantId").CurrentValue}]");
```

* Run and show that things work