# Demo: OwnedEntities

## Everytime Setup

Required before/after each run of the demo
* Reset the starting point source code (`git reset --hard`)

## Demo Steps

### Part 1: Natural Keys

* Set `SeedData` as the startup project.
* Walk thru the model and show that there are migrations
* Add some seed data for Tags

```c#
modelBuilder.Entity<Tag>().SeedData(
    new Tag { TagId = ".NET" },
    new Tag { TagId = "VisualStudio" },
    new Tag { TagId = "C#" },
    new Tag { TagId = "F#" },
    new Tag { TagId = "VB.NET" },
    new Tag { TagId = "EnityFramework" },
    new Tag { TagId = "ASP.NET" });
```

* `Add-Migration TagData`
* Walk thru scaffolded migration
* `Update-Database`
* Show data in the database

### Part 2: Generated Keys

* Add some seed data for Users

```c#
modelBuilder.Entity<User>().SeedData(
    new User { UserId = 1, UserName = "guest" },
    new User { UserId = 2, UserName = "administrator" });
```

* `Add-Migration UserData`
* Walk thru scaffolded migration
* `Update-Database`
* Show data in the database

### Part 3: Updating Data

* Change the administrator name to be capitialized
* Remvoe the guest user

```c#
modelBuilder.Entity<User>().SeedData(
    new User { UserId = 2, UserName = "Administrator" });
```

* `Add-Migration FixUserData`
* Walk thru scaffolded migration
* `Update-Database`
* Show data in the database