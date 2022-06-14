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
    [Table("Courier")]
    public class Courier :  Person
    {
        public Courier(string name, string surname, string patronymic, DateTime birth, Organization organization) : base(name, surname, patronymic, birth)
        {
            Rating = 2.5;//начальный рейтинг усреднен
            Organization = organization;
        }

        public Courier() : base(null, null, null, new DateTime())
        {
            Rating = 2.5;//начальный рейтинг усреднен
        }

        public override string Name { get; set; }
        public override string Surname { get; set; }
        public override string Patronymic { get; set; }
        public override DateTime Birth { get;  set; }

        [Key]
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int? AddressId { get; set; }
        public Address Address { get; set; }
        public bool IsActivity { get; set; }

        public double Rating { get; private set; }
        public readonly AccessRights LevelAccess = AccessRights.Courier;
        private string numberPhone;
        public string NumberPhone
        {
            get => numberPhone;
            set => numberPhone = Other.CheckNumberPhone(value);
        }

        public void UpdateRating()
        {
            //TODD: метод обновления рейтинга курьера   
        }
        public override string ToString()
        {
            return $"КУРЬЕР: ФИО: \"{Name} {Surname} {Patronymic}\" Возраст: {Age} Организация: \"{Organization.Name}\"";
        }

        public Courier FindInDb()
        {
            Courier dbCourier;
            using (var db = new DeliveryContext())
            {
                dbCourier = db.Courier
                    .Where(c => c.Surname.Equals(Surname) && c.Birth == Birth)
                    .Include(c => c.Organization)
                    .FirstOrDefault();
            }
            return dbCourier;
        }
        public void AddInDB()
        {
            using (var db = new DeliveryContext())
            {
                Organization = db.Organization.Find(OrganizationId);
                Address = db.Address.Find(AddressId);
                db.Courier.Add(this);
                db.SaveChanges();
            }
        }
        public void UpdateInDB()
        {
            using (var db = new DeliveryContext())
            {
                var actual = db.Courier.Find(Id);
                db.Entry(actual).State = EntityState.Modified;
                actual.Name = Name;
                actual.Surname = Surname;
                actual.Patronymic = Patronymic;
                actual.Birth = Birth;
                actual.IsActivity = IsActivity;
                actual.Organization = db.Organization.Find(OrganizationId);
                actual.NumberPhone = NumberPhone;
                actual.Address = db.Address.Find(AddressId);
                actual.Rating = Rating;
                db.SaveChanges();
            }
        }

        public void MakeActivity(bool activity)
        {
            using (var db = new DeliveryContext())
            {
                var dbcourier = db.Courier
                    .Include(c => c.Organization)
                    .First(c => c.Id == Id);
                dbcourier.IsActivity = activity;
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
                while (true)
                {
                    if (OrganizationId == null)
                    {
                        if (Organization == null)
                        {
                            List<Organization> orgs;
                            using (var db = new DeliveryContext())
                            {
                                orgs = db.Organization
                                     .AsNoTracking()//ускорение
                                     .ToList();
                            }
                            Console.WriteLine("Выберите вашу организацию (по номеру):");
                            orgs.ForEach(org => Console.WriteLine($"{org.Id}-{org.Name}"));
                            var num = int.Parse(Console.ReadLine());

                            Organization org;
                            using (var db = new DeliveryContext())
                            {
                                org = db.Organization.FirstOrDefault(org => org.Id == num);
                                //db.Dispatcher.Find(this.Id).OrganizationId = org.Id;
                                //db.SaveChanges();
                            }
                            if (org == null)
                            {
                                Console.WriteLine("Ошибка ввода номера.");
                                continue;
                            }
                            Organization = org;
                            OrganizationId = org.Id;

                            break;
                        }
                        else
                        {
                            var regOrg = Tools.FindInDB(Organization);
                            if (!regOrg.Equals(default) && regOrg != null)
                            {
                                Organization = regOrg;
                                break;
                            }
                            Console.WriteLine("Такая организация не зарегестрирвоана в базе данных, попробуйте ввести другую организацию или зарегестрируйте новую через администратора.", ConsoleColor.DarkRed);
                        }
                    }
                    else
                    {
                        Organization org;
                        using (var db = new DeliveryContext())
                        {
                            org = db.Organization.Find(OrganizationId);
                        }
                        Organization = org;
                        break;
                    }
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
