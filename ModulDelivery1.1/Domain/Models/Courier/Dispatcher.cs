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
    [Table("Dispatcher")]
    public class Dispatcher : Person
    {
        public Dispatcher(string name, string surname, string patronymic, DateTime birth, Organization organization) : base(name, surname, patronymic, birth)
        {
            Organization = organization;
        }
        public Dispatcher() : base(null, null, null, new DateTime())
        {
            //Organization = new Organization();
        }

        public override string Name { get; set; }
        public override string Surname { get; set; }
        public override string Patronymic { get; set; }
        public override DateTime Birth { get; set; }

        [Key]
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public bool IsActivity { get; set; }

        public readonly AccessRights LevelAccess = AccessRights.Dispatcher;
        private string numberPhone;
        public string NumberPhone
        {
            get => numberPhone;
            set => numberPhone = Other.CheckNumberPhone(value);
        }
        /// <summary>
        /// Находит продавцов, которые имеют в наличии искомый продукт
        /// </summary>
        /// <param name="product">Искомый продукт</param>
        /// <param name="sellers">Предпочительные продавцы</param>
        /// <returns>Возвращает перечисление продавцов</returns>
        public IEnumerable<Seller> GetSellerHavingProducts(List<Product> products, params Seller[] sellers)
        {
            if (sellers == null || sellers.Length == 0)
            {
                sellers = Seller.GetAllSeller().ToArray();
            }
            return sellers.Where(s => s.IsGiveProduct(products));
        }


        public List<Tuple<Courier, Warehouse>> MakeDelivery(List<Product> products, Seller seller, Customer customer)
        {
            Seller dbSeller;
            using (var db = new DeliveryContext())
            {
                var dbWarehouses = db.Warehouse
                    .Include(w => w.ListProduct)
                    .Include(w=>w.Address);
                dbSeller = db.Seller
                    //.Include(s => s.Organization)
                    .Include(s => s.Organization.ListWarehouse)
                    .Where(s => s.Id == seller.Id)
                    .ToList()
                    //.ForEach(s => s.Organization.ListWarehouse =dbWarehouses.Where(w => w.OrganizationId == s.OrganizationId).ToList());
                    .FirstOrDefault();
                dbSeller.Organization.ListWarehouse=dbWarehouses.Where(w => w.OrganizationId == dbSeller.OrganizationId).ToList();
                //TODO: не заполняется ListWarehouse.ListProduct
            }
            var warehouses = dbSeller.Organization.ListWarehouse;//все склады выбранного продавца
            var groupProducts = new List<Tuple<Warehouse, List<Product>>>();

            var temp = new Product[products.Count()];//копирование, чтобы не изменять ссылки переданных переменных
            products.CopyTo(temp);
            var productsCopy = temp.ToList();

            foreach (var w in warehouses)
            {
                if (w.ListProduct == null || w.ListProduct.Count() == 0)
                    continue;
                var prods = productsCopy
                    .Where(p => w.ListProduct.FirstOrDefault(pp => pp.Id == p.Id) != null)
                    .ToList();
                if (prods.Count() > 0)
                {
                    var group = new Tuple<Warehouse, List<Product>>(w, prods);
                    groupProducts.Add(group);
                }
                prods.ForEach(p => productsCopy.Remove(p));
            }


            var linkCourierWare = new List<Tuple<Courier, Warehouse>>();
            using (var db = new DeliveryContext())
            {
                foreach(var group in groupProducts)
                {
                    var orderCouriers = db.Courier
                        .Include(c => c.Address)
                        .Include(c=>c.Organization)
                        .ToList()
                        .Where(c => c.IsActivity)
                        .OrderByDescending(c => c.Address.Equals(group.Item1.Address))
                        .ToList();
                    if (orderCouriers == null)
                        break;
                    var cour = orderCouriers.First();
                    var ware = group.Item1;
                    var cw = new Tuple<Courier, Warehouse>(cour, ware);
                    linkCourierWare.Add(cw);
                }
            }
            if (linkCourierWare.Count() == 0)
                return null;//нет активных курьеров
            //в лист продуктов склада пололжим продукты, которые от туда будут доставлены
            for (var i = 0; i < linkCourierWare.Count(); i++)
            {
                linkCourierWare[i].Item2.ListProduct = groupProducts[i].Item2;
            }
            return linkCourierWare;
        }


        public Courier MakeDelivery(Seller[] sellers, List<Product> products, Customer customer)
        {
            var sel = GetSellerHavingProducts(products, sellers);
            return СreateDelivery(sel, products, customer);
        }
        private Courier СreateDelivery(IEnumerable<Seller> sellers, List<Product> products, Customer customer)
        {
            IEnumerable<Courier> couriers;
            using (var db = new DeliveryContext())
            {
                couriers = db.Courier
                    .Where(c => c.IsActivity);
                    //.AsNoTracking();//ускорение
            }

            var del = new Delivery();
                
            Tuple<Warehouse, TimeSpan> minTimeDel = null;
            foreach (var s in sellers)
            {
                var temp = del.GetTimeDelivery(products, s, customer);
                if(minTimeDel != null && temp.Item2 < minTimeDel.Item2)
                    minTimeDel = temp;
            }

            return couriers.First();
        }
        public override string ToString()
        {
            return $"ДИСПЕТЧЕР: ФИО: \"{Name} {Surname} {Patronymic}\" Возраст: {Age} Организация: \"{Organization.Name}\"";
        }

        public Dispatcher FindInDb()
        {
            Dispatcher dbDispatcher;
            using (var db = new DeliveryContext())
            {
                dbDispatcher = db.Dispatcher
                .Where(s => s.Surname.Equals(Surname) && s.Birth == Birth)
                .Include(c => c.Organization)
                .FirstOrDefault();
            }
            return dbDispatcher;
        }
        public void AddInDB()
        {
            using (var db = new DeliveryContext())
            {
                Organization = db.Organization.Find(OrganizationId);
                db.Dispatcher.Add(this);
                db.SaveChanges();
            }
        }
        public void UpdateInDB()
        {
            using (var db = new DeliveryContext())
            {
                var actual = db.Dispatcher.Find(Id);
                db.Entry(actual).State = EntityState.Modified;
                actual.Name = Name;
                actual.Surname = Surname;
                actual.Patronymic = Patronymic;
                actual.Birth = Birth;
                actual.IsActivity = IsActivity;
                actual.Organization = db.Organization.Find(OrganizationId);
                actual.NumberPhone = NumberPhone;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Нацти продавцов, у которых есть в наличии искомый список товаров
        /// </summary>
        /// <param name="products">Искомый список товаров</param>
        /// <returns>Список продавцов имеющих товар</returns>
        public List<Seller> FindSellersHaving(List<Product> products)
        {
            List<Seller> sellers;
            sellers = GetSellerHavingProducts(products)
                .ToList();
            return sellers;
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
            }
        }
    }
}
