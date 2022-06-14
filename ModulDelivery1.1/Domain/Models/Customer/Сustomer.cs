using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using ModulDelivery.Infrastructure;

namespace ModulDelivery.Domain.Models
{
    [Table("Customer")]
    public class Customer : Person
    {
        public Customer(string name, string surname, string patronymic, DateTime birth, Address address) : base(name, surname, patronymic, birth)
        {
            Address = address;
        }
        public Customer() : base(null, null, null, new DateTime())
        {
            //Address = new Address();
        }

        public override string Name { get ;   set; }
        public override string Surname { get ;  set ; }
        public override string Patronymic { get ;  set ; }
        public override DateTime Birth { get ;  set ; }

        [Key]
        public int Id { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }
        public readonly AccessRights LevelAccess = AccessRights.Customer;
        private string numberPhone;
        public string NumberPhone
        {
            get => numberPhone;
            set => numberPhone = Other.CheckNumberPhone(value);
        }


        public List<Tuple<Courier, Warehouse>> BuyProduct(List<Product> products, Seller seller)
        {
            var delivery = new Delivery();
            var dispatcher = delivery.GetDispatcher();
            if (dispatcher == null)
                return null;

            return dispatcher.MakeDelivery(products, seller, this);
        }


        public override string ToString()
        {
            return $"ПОКУПАТЕЛЬ: ФИО: \"{Name} {Surname} {Patronymic}\" Возраст: {Age}";
        }

        public Customer FindInDb()
        {
            Customer dbCustomer;
            using (var db = new DeliveryContext())
            {
                dbCustomer = db.Customer
                    .Where(c => c.Surname == Surname && c.Birth == Birth)
                    .Include(c => c.Address)
                    .FirstOrDefault();
            }
            return dbCustomer;
        }
        public void AddInDB()
        {
            using (var db = new DeliveryContext())
            {
                Address = db.Address.Find(AddressId);
                db.Customer.Add(this);
                db.SaveChanges();
            }
        }
        public void UpdateInDB()
        {
            using (var db = new DeliveryContext())
            {
                var actual = db.Customer.Find(Id);
                db.Entry(actual).State = EntityState.Modified;
                actual.Name = Name;
                actual.Surname = Surname;
                actual.Patronymic = Patronymic;
                actual.Birth = Birth;
                actual.Address = db.Address.Find(AddressId);
                actual.NumberPhone = NumberPhone;
                db.SaveChanges();
            }
        }

        public void UserInitialization(bool isFull)
        {
            UserInitializationPerson(isFull, this);
            if (isFull)
            {
                if (string.IsNullOrEmpty(this.NumberPhone))
                {
                    Console.Write("Ваш номер телефона: ");
                    this.NumberPhone = Console.ReadLine().Trim().ToLower();
                }
                if (AddressId == null)
                {
                    if (Address == null)
                    {
                        Address dbAddress;
                        var addr = new Address();
                        addr.UserInitialization();
                        using (var db = new DeliveryContext())
                        {
                            dbAddress = db.Address
                                .Where(a => a.Country == addr.Country
                                && a.City == addr.City
                                && a.Street == addr.Street
                                && a.House == addr.House)
                                .FirstOrDefault();
                        }
                        if (dbAddress == null)
                        {
                            using (var db = new DeliveryContext())
                            {
                                db.Address.Add(addr);
                                db.SaveChanges();
                                Address = addr;
                                //db.Customer.Find(this.Id).Address = addr;
                            }
                        }
                        else
                        {
                            //using (var db = new DeliveryContext())
                            //{
                            //    db.Customer.Find(this.Id).Address = dbAddress;
                            //}
                            Address = dbAddress;
                            AddressId = dbAddress.Id;
                        }
                    }
                    else
                        this.AddressId = Address.Id;
                }
                else
                {
                    if (Address == null)
                    {
                        Address dbAddress;
                        using (var db = new DeliveryContext())
                        {
                            dbAddress = db.Address
                                .Find(AddressId);
                            //db.Customer.Find(this.Id).Address = dbAddress;
                        }
                        Address = dbAddress;
                    }
                }
            }
        }
    }
}
