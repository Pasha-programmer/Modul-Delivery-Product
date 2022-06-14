using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ModulDelivery.Infrastructure;
using System.Data.Entity;

namespace ModulDelivery.Domain.Models
{
    [Table("Seller")]
    public class Seller : Person
    {
        public Seller(string name, string surname, string patronymic, DateTime birth, Organization organization) : base(name, surname, patronymic, birth)
        {
            Organization = organization;
        }
        public Seller() : base(null, null, null, new DateTime())
        {
            //Organization = new Organization();
        }

        public override string Name { get;  set; }
        public override string Surname { get;  set; }
        public override string Patronymic { get;  set; }
        public override DateTime Birth { get;  set; }

        [Key]
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public readonly AccessRights LevelAccess = AccessRights.Seller;
        private string numberPhone;
        public string NumberPhone
        {
            get => numberPhone;
            set => numberPhone = Other.CheckNumberPhone(value);
        }

        /// <summary>
        /// Получение всех складов у продавца имеющих искомые товары
        /// </summary>
        /// <param name="products">Искомые продукты</param>
        /// <returns>Возвращает список продуктов с информацию для отслеживания</returns>
        public List<Product> GetWarehouseHaving(List<Product> products)
        {
            var res = new List<Product>();
            using (var db = new DeliveryContext())
            {
                //находим в бд организацию продавца
                var org = db.Organization
                    .Find(OrganizationId);//Where(org => org.Id == OrganizationId).First();
                //находим все склады у организации
                var ws = db.Warehouse
                    .Where(w => w.OrganizationId == org.Id)
                    .ToList();
                foreach (var w in ws)
                {
                    foreach(var product in products)
                    {
                        var pWA = db.Product
                            .Where(p => p.Id == product.Id)
                            .Include(p => p.WareHouse.Address)//соедниение таблиц склада и адресса
                            .Where(p => p.WareHouse.Id == w.Id);
                        if (pWA.Count() > 0)
                            res.Add(pWA.First());
                        if (pWA.Count() > 1) throw new ArgumentException("ОПАААА");//TODO check and delete
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Получение всех товаров у продавца
        /// </summary>
        /// <param name="products">Тип искомого товара</param>
        /// <returns>Возвращает перечисление продуктов искомого типа</returns>
        //private IEnumerable<Product> GetProduct(IEnumerable<Product> products)
        //{
        //    List<Product> res = new List<Product>();
        //    List<Warehouse> wss;
        //    //wss = GetWarehouseHaving(products);
        //    using (var db = new DeliveryContext())
        //    {
        //        var org = db.Organization
        //            .Where(org => org.Id == OrganizationId).First();
        //        wss = db.Warehouse
        //            .Where(w => w.OrganizationId == org.Id)
        //            .ToList();
        //        //using (var db = new DeliveryContext())
        //        //{
        //        //    var o = db.Organization
        //        //        .Where(org => org.Id == OrganizationId).First();
        //        //    var ws = db.Warehouse
        //        //        .Where(w => w.OrganizationId == o.Id);
        //        //    wss = db.Product
        //        //        .SelectMany(p => ws.Where(ww => ww.Id == p.WareHouseId))
        //        //        .ToList();
        //        //}
        //        res = wss.SelectMany(w => w.GetProduct(products.First()))
        //        .ToList();
        //    }
        //    return res;
        //}

        public bool IsGiveProduct(List<Product> products)
        {
            return GetWarehouseHaving(products).Count()>0;
        }
        public bool IsGiveProduct(Product product)
        {
            return GetWarehouseHaving(new List<Product> { product }).Count() > 0;
        }
        public override string ToString()
        {
            return $"ПРОДАВЕЦ: ФИО: \"{Name} {Surname} {Patronymic}\" Возраст: {Age} Организация: \"{Organization.Name}\"";
        }

        public static List<Seller> GetAllSeller()
        {
            List<Seller> sellers;
            using (var db = new DeliveryContext())
            {
                sellers = db.Seller
                    .Include(s => s.Organization)
                    .AsNoTracking()//ускорение
                    .ToList();
            }
            return sellers;
        }

        /// <summary>
        /// Поиск продавца в БД по фамилии и дате рождения
        /// </summary>
        /// <returns></returns>
        public Seller FindInDb()
        {
            Seller dbSeller;
            using (var db = new DeliveryContext())
            {
                dbSeller = db.Seller
                    .Where(s => s.Surname.Equals(Surname) && s.Birth == Birth)
                    .Include(s => s.Organization)
                    .FirstOrDefault();
            }
            return dbSeller;
        }
        public void AddInDB()
        {
            using (var db = new DeliveryContext())
            {
                Organization = db.Organization.Find(OrganizationId);
                db.Seller.Add(this);
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
                actual.Organization = db.Organization.Find(OrganizationId);
                actual.NumberPhone = NumberPhone;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Пользовательская инициализация объекта
        /// </summary>
        /// <param name="isFull">Полная инициализация или нет</param>
        public void UserInitialization(bool isFull)
        {
            //Seller seller = new Seller();
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
            }
        }
    }
}
