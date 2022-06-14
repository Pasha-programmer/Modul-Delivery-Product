using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ModulDelivery.Infrastructure;
using System.Data.Entity;

namespace ModulDelivery.Domain.Models
{
    [Table("Food")]
    public class Product
    {
        public Product(string name, decimal price, string preview)
        {
            Name = name;
            Price = price;
            Preview = preview;
        }
        public Product() 
        {
            //WareHouse = new Warehouse();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Preview { get; set; }
        public int? WareHouseId { get; set; }
        public Warehouse WareHouse { get; set; }
        public override string ToString()
        {
            var preview = this.Preview == null ? null : $"Описание: {this.Preview}";
            return $"Наименование: {this.Name} Цена: {this.Price}" + " " + preview;
        }
        /// <summary>
        /// Получение всех продуктов группированых по принадлежности к организации
        /// </summary>
        /// <returns>Список групп организаций с л=списками имеющихся продуктов</returns>
        public static List<IGrouping<Organization, Product>> GetProducts()
        {
            List<IGrouping<Organization, Product>> groupProducts;
            using (var db = new DeliveryContext())
            {
                groupProducts = db.Product
                    .Include(p => p.WareHouse.Organization.ListSeller)
                    .OrderBy(p => p.Name)
                    .Include(p => p.WareHouse.Address)
                    .ToList()
                    .GroupBy(p => p.WareHouse.Organization)
                    .ToList();
            }
            return groupProducts;
        }

        public static List<Product> FindInDBByName(string productName)
        {
            List<Product> prods;
            using (var db = new DeliveryContext())
            {
                prods = db.Product
                    .Where(p => p.Name == productName)
                    .Include(p => p.WareHouse.Organization)
                    .ToList();
            }
            return prods;
        }
    }
}
