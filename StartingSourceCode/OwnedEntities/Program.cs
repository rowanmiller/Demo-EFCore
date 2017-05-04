using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace OwnedEntities
{
    class Program
    {
        static void Main(string[] args)
        {
            RecreateDatabase();

            Console.Write(" Inserting Data......");

            using (var db= new BloggingContext())
            {
                var usa = new Country { Id = "USA", Name = "United States of America" };
                db.Add(usa);

                db.Customers.Add(new Customer
                {
                    Name = "Rowan",
                    WorkAddress = new Address
                    {
                        LineOne = "Microsoft",
                        LineTwo = "One Microsoft Way",
                        Location = new Location
                        {
                            CityOrTown = "Redmond",
                            PostalOrZipCode = "98052",
                            StateOrProvince = "WA"
                        },
                        Country = usa
                    },
                    PhysicalAddress = new Address
                    {
                        LineOne = "Washington State Convention Center",
                        LineTwo = "705 Pike St",
                        Location = new Location
                        {
                            CityOrTown = "Seattle",
                            PostalOrZipCode = "98101",
                            StateOrProvince = "WA"
                        },
                        Country = usa
                    }
                });

                db.SaveChanges();
            }

            Console.WriteLine(" done");
        }

        private static void RecreateDatabase()
        {
            using (var db = new BloggingContext())
            {
                Console.WriteLine("Recreating database from current model");
                Console.Write(" Dropping database...");
                db.Database.EnsureDeleted();
                Console.WriteLine(" done");

                Console.Write(" Creating database...");
                db.Database.EnsureCreated();
                Console.WriteLine(" done");
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.OwnedEntities;Trusted_Connection=True;ConnectRetryCount=0;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .OwnsOne(c => c.WorkAddress)
                    .OwnsOne(a => a.Location);

            modelBuilder.Entity<Customer>()
                .OwnsOne(c => c.PhysicalAddress)
                    .ToTable("Customers_Location")
                    .OwnsOne(a => a.Location);
        }
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }

        public Address WorkAddress { get; set; }
        public Address PhysicalAddress { get; set; }
    }

    public class Address
    {
        public string LineOne { get; set; }
        public string LineTwo { get; set; }
        public Location Location { get; set; }

        public string CountryId { get; set; }
        public Country Country { get; set; }
    }

    public class Location
    {
        public string PostalOrZipCode { get; set; }
        public string StateOrProvince { get; set; }
        public string CityOrTown { get; internal set; }
    }

    public class Country
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
