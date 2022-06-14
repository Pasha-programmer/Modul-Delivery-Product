using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using ModulDelivery.Domain.Models;

namespace ModulDelivery.Infrastructure
{
    public class DeliveryContext : DbContext
    {
        public DeliveryContext() : base("Delivery") { } 

        public DbSet<Organization> Organization { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Courier> Courier { get; set; }
        public DbSet<Dispatcher> Dispatcher { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Seller> Seller { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Address> Address { get; set; }
    }
}
