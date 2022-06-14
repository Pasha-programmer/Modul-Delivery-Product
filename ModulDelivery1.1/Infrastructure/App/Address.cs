using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ModulDelivery.Infrastructure
{
    [Table("Address")]
    public class Address
    {
        public Address(string country, string city, string street, string house)
        {
            Country = country;
            City = city;
            Street = street;
            House = house;
        }
        public Address() { }

        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }

        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Возвращает расстояние между двумя адрессами 
        /// </summary>
        /// <param name="finishAddress"></param>
        /// <returns></returns>
        public int GetDistanse(Address finishAddress)
        {
            //TODO: реализовать метод получения расстояния между двумя адрессами
            return new Random().Next() * 100;//прмиерное расстояние в километрах
        }
        public override string ToString()
        {
            return $"AДРЕСС: {Country}, {City}, {Street}, {House}";
        }

        /// <summary>
        /// Оценивает близость двух адрессов
        /// </summary>
        /// <param name="address">сравнимый адресс</param>
        /// <returns>0 - в разных странах\n1 - в разных городах\n2 - на разных улицах\n3 - в разных домах\n4 - в одном доме\n</returns>
        public int Equals(Address address)
        {
            if (address == null)
                throw new ArgumentNullException();
            int level = 0;
            if (address.Country == Country)
            {
                level++;
                if (address.City == City)
                {
                    level++;
                    if (address.Street == Street)
                    {
                        level++;
                        if (address.House == House)
                            level++;
                    }
                }
            }
            return level;
        }

        public void UserInitialization()
        {
            Console.WriteLine("Ваш адресс:");
            Console.Write("Страна:");
            this.Country = Console.ReadLine().Trim().ToLower();
            Console.Write("Город:");
            this.City = Console.ReadLine().Trim().ToLower();
            Console.Write("Улица:");
            this.Street = Console.ReadLine().Trim().ToLower();
            Console.Write("Дом:");
            this.House = Console.ReadLine().Trim().ToLower();
        }
    }
}
