# Demo: OwnedEntities

## Everytime Setup

Required before/after each run of the demo
* Reset the starting point source code (`git reset --hard`)


## Demo Steps

### Part 1: Basic Complex Types

* Set `OwnedEntities` as the startup project.
* Walk thru the model
* Run and show exception because `Address` doesn't have a key
* Update `Address` to be an owned entity

```c#
 modelBuilder.Entity<Customer>()
    .OwnsOne(c => c.WorkAddress);

modelBuilder.Entity<Customer>()
    .OwnsOne(c => c.PhysicalAddress);
```

* Run app then show schema

### Part 2: Splitting Into Tables

* Split `PhysicalAddress` out to separate table

```c#
modelBuilder.Entity<Customer>()
    .OwnsOne(c => c.PhysicalAddress)
        .ToTable("Customers_Location");
```

* Run app then show schema

### Part 3: Nested

* Move some fields to a `Location` class

```c#
public class Address
{
    public string LineOne { get; set; }
    public string LineTwo { get; set; }
    public Location Location { get; set; }
}

public class Location
{
    public string PostalOrZipCode { get; set; }
    public string StateOrProvince { get; set; }
    public string CityOrTown { get; internal set; }
    public string Country { get; internal set; }
}
```

* Update `OnModelCreating(...)` to include the nested ownership

```c#
modelBuilder.Entity<Customer>()
    .OwnsOne(c => c.WorkAddress)
        .OwnsOne(a => a.Location);

modelBuilder.Entity<Customer>()
    .OwnsOne(c => c.PhysicalAddress)
        .ToTable("Customers_Location")
        .OwnsOne(a => a.Location);
```

* Update `Main()` to reflect the new structure
* Run app then show schema

### Part 4: Navigation Properties

* Split `Country` out into an entity

```c#
 public class Location
{
    public string PostalOrZipCode { get; set; }
    public string StateOrProvince { get; set; }
    public string CityOrTown { get; internal set; }

    public string CountryId { get; set; }
    public Country Country { get; set; }
}

public class Country
{
    public string Id { get; set; }
    public string Name { get; set; }
}
```

* Update `Main()` to create a new `Country` and assign it to both `Location`s

```c#
var usa = new Country { Id = "USA", Name = "United States of America" };
db.Add(usa);
```

* Run app then show schema