using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace OwnedEntities
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new CustomerContext())
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

    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.OwnedEntities;Trusted_Connection=True;ConnectRetryCount=0;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

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
        public string PostalOrZipCode { get; set; }
        public string StateOrProvince { get; set; }
        public string CityOrTown { get; internal set; }
        public string CountryName { get; set; }
    }
}
