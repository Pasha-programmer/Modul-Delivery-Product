using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ModulDelivery.Domain.Models
{
    [Table("Food")]
    public class Food : Product
    {
        public Food(string name, decimal price, string preview)
        {
            Name = name;
            Price = price;
            Preview = preview;
        }
        public Food() { }

        [Key]
        public int Id { get; set; }
        public int? WareHouseId { get; set; }
        public Warehouse WareHouse { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                value = value.Trim();
                if (value.Length != 0)
                    name = value;
                else
                    throw new ArgumentNullException("Попытка записать в название товара пустую строку.");
            }
        }
        private decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                if (value >= 0)
                    price = value;
                else
                    throw new ArgumentException("Попытка записать в стоимость товара сумму меньше нуля.");
            }
        }
        public string preview;
        public string Preview
        {
            get => name;
            set
            {
                value = value.Trim();
                if (value.Length != 0)
                    preview = value;
                else
                    throw new ArgumentNullException("Попытка записать в название товара пустую строку.");
            }
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
