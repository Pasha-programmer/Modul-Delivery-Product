using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ModulDelivery.Domain.Models
{
    [Table("Organization")]
    public class Organization : IOwner
    {
        public Organization(string name)
        {
            Name = name;
        }
        public Organization() 
        {
            ListDispatcher = new List<Dispatcher>();
            ListCourier = new List<Courier>();
            ListSeller = new List<Seller>();
            ListWarehouse = new List<Warehouse>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; private set; }
        public ICollection<Dispatcher> ListDispatcher { get; set; }
        public ICollection<Courier> ListCourier { get; set; }
        public ICollection<Seller> ListSeller { get; set; }

        public ICollection<Warehouse> ListWarehouse { get; set; }

        public override string ToString()
        {
            return $"ОРГАНИЗАЦИЯ: Наименование \"{Name}\"";
        }

        public void UserInitialization()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Console.Write("Наименвоание организации: ");
                this.Name = Console.ReadLine().Trim().ToLower();
            }
        }
    }
}
