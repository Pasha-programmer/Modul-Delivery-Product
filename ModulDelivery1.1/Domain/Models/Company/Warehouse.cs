using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ModulDelivery.Infrastructure;

namespace ModulDelivery.Domain.Models
{
    [Table("Warehouse")]
    public class Warehouse
    {
        public Warehouse(Address address, Organization owner)
        {
            Address = address;
            Organization = owner;
        }

        public Warehouse(Address address, Organization owner,  WorkSchedule workSchedule, TimeSpan workingTimeTo) : this(address, owner)
        {
            WorkingTime = workSchedule;
        }

        public Warehouse() 
        {
            ListProduct = new List<Product>();
            //Organization = new Organization();
            //Address = new Address();
        }

        [Key]
        public int Id { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public ICollection<Product> ListProduct { get; set; }
        public WorkSchedule WorkingTime { get; }
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }

        /// <summary>
        /// Получить искомые продукты на складе
        /// </summary>
        /// <param name="product">Искомый продукт</param>
        /// <returns>Возвращает перечисление продуктов</returns>
        public List<Product> GetProduct(Product product)
        {
            List<Product> products;
            using (DeliveryContext db = new DeliveryContext())
            {
                products = db.Product.Where(p => p.WareHouseId == this.Id && p.Name == product.Name)
                    .ToList();
            }
            return products;
        }
        /// <summary>
        /// Проверка рабоатет ли склад сейчас
        /// </summary>
        /// <returns>true - рабаотает, иначе false</returns>
        internal bool IsWorkingTime()
        {
            var nowTime = DateTime.Now;
            var scheduleNow = WorkingTime.schedule[nowTime.DayOfWeek];
            return (nowTime.Hour >= scheduleNow.From.Hour &&
                nowTime.Hour < scheduleNow.To.Hour &&
                nowTime.Minute >= scheduleNow.From.Minute &&
                nowTime.Minute < scheduleNow.To.Minute);
        }
        public override string ToString()
        {
            return $"СКЛАД: по адрессу: \"{Address}\" Cобственник: \"{Organization.Name}\"";
        }
    }
}
