/*

// Part 1.2: Tenant Query Filter 
modelBuilder.Entity<Blog>()
    .HasQueryFilter(b => b.TenantId == _tenantId);

// Part 2.1: Tenant Field
modelBuilder.Entity<Blog>()
    .Property<string>("TenantId")
    .HasField("_tenantId")
    .Metadata.IsReadOnlyAfterSave = true;

modelBuilder.Entity<Blog>()
    .HasQueryFilter(b => EF.Property<string>(b, "TenantId") == _tenantId);
        
// Part 2.2: IsDeleted Shadow Property
modelBuilder.Entity<Post>()
    .Property<bool>("IsDeleted");

modelBuilder.Entity<Post>()
    .HasQueryFilter(p => !EF.Property<bool>(p, "IsDeleted"));

// Part 7.1: Tag Seed Data
                new Tag { TagId = ".NET" },
                new Tag { TagId = "VisualStudio" },
                new Tag { TagId = "C#" },
                new Tag { TagId = "F#" },
                new Tag { TagId = "VB.NET" },
                new Tag { TagId = "EnityFramework" },
                new Tag { TagId = "ASP.NET" }

// Part 7.2: User Seed Data
                new User { UserId = 1, UserName = "guest" },
                new User { UserId = 2, UserName = "administrator" }

*/