using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using ModulDelivery;
using ModulDelivery.Domain.Models;
using ModulDelivery.Infrastructure;
using System.Data.Entity;

namespace ModulDelivery.App
{
    public class DeliveryConsoleUi
    {
		private void GetWelcomeMessage()
        {
			Console.WriteLine("Выберите ваш тип профиля:");
			var attribute = typeof(AccessRights)
				.GetTypeInfo()
				.DeclaredFields
				.Skip(1)
				.Select(x => x.GetCustomAttribute<LabelAttribute>().name)
				.ToList();
			attribute.ForEach(attr => Console.WriteLine(" " + attr));
			Console.WriteLine();
		}
		public void Run()
		{
			//определение права доступа
			Console.WriteLine("Добро пожаловать в службу доставки!");
			while (true)
			{
				GetWelcomeMessage();
				var profile = Console.ReadLine().ToUpper().Trim();
				switch (profile)
				{
					case "КЛИЕНТ":
						var customer = FindCustomerDB();
						MakeWorkCustomer(customer);
						continue;

					case "КУРЬЕР":
						var courier = FindCourierDB();
						MakeWorkCourier(courier);
						continue;

					case "ДИСПЕТЧЕР":
						var dispatcher = FindDispatcherDB();
						MakeWorkDispatcher(dispatcher);
						continue;

					case "ПРОДАВЕЦ":
						var seller = FindSellerDB();
						MakeWorkSeller(seller);
						continue;

					//case "АДМИНИСТРАТОР":
					//	MakeWorkAdmin();
					//	break;

					default:
						Console.WriteLine("Неизвестный профиль: ");
						continue;
				}
			}
		}

		private Customer FindCustomerDB()
		{
			var customer = new Customer();
			Customer dbCustomer;
			customer.UserInitialization(false);
			dbCustomer = customer.FindInDb();
			if (dbCustomer == null)
			{
				customer.UserInitialization(true);
				customer.AddInDB();
			}
			else
			{
				dbCustomer.UserInitialization(true);
				dbCustomer.UpdateInDB();
				customer = dbCustomer;
			}
			return customer;
		}
		private Courier FindCourierDB()
		{
			var courier = new Courier();
			Courier dbCourier;
			courier.UserInitialization(false);
			dbCourier = courier.FindInDb();
			if (dbCourier == null)
			{
				courier.UserInitialization(true);
				courier.AddInDB();
			}
			else
			{
				dbCourier.UserInitialization(true);
				dbCourier.UpdateInDB();
				courier = dbCourier;
			}
			return courier;
		}
		private Dispatcher FindDispatcherDB()
		{
			var dispatcher = new Dispatcher();
			Dispatcher dbDispatcher;
			dispatcher.UserInitialization(false);
			dbDispatcher = dispatcher.FindInDb();
			if (dbDispatcher == null)
			{
				dispatcher.UserInitialization(true);
				dispatcher.AddInDB();
			}
			else
			{
				dbDispatcher.UserInitialization(true);
				dbDispatcher.UpdateInDB();
				dispatcher = dbDispatcher;
			}
			return dispatcher;
		}
		private Seller FindSellerDB()
        {
			var seller = new Seller();
			Seller dbSeller;
			seller.UserInitialization(false);
			dbSeller = seller.FindInDb();
			if (dbSeller == null)
			{
				seller.UserInitialization(true);
				seller.AddInDB();
			}
			else
			{
				dbSeller.UserInitialization(true);
				dbSeller.UpdateInDB();
				seller = dbSeller;
			}
			return seller;
		}

		private void MakeWorkCustomer(Customer customer)
        {
			var defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			const char buyProduct = 'B';
			const char exit = 'E';
			while (true)
			{
				bool codeExit = false;
				var buttons = new Dictionary<char, string>()
				{
					{ buyProduct, "заказать продукт с доставкой" },
					{ exit, "выйти из профиля клиента" }
				};
				var choice = GetPromptChoice(buttons);//считать клавишу введенную пользователем

				switch (choice) {
					case (buyProduct)://заказать продукт с доставкой

						//выгрузка всех продуктов	
						List<IGrouping<Organization, Product>> groupProducts = Product.GetProducts();

						var basket = new List<Product>();//корзина товаров
						Console.WriteLine("(Для оформления заказа напишите \"купить\")");
						Console.WriteLine("Выберите продукт:");
						var numId = new List<int>();
						foreach (var g in groupProducts)
						{
							Console.WriteLine($"Продавец \"{g.Key.Name}\"");
							foreach (var p in g)
							{
								numId.Add(p.Id);
								Console.WriteLine($" {numId.Count()}. {p}");//numId.Count()-1 индекс товара, по которому в numId хранится Id товара
							}
						}

						Organization selectOrganization = null;
						while (true)
						{
							var choise = Console.ReadLine().Trim().ToLower();
							if (choise == "купить")
								break;
							int num;
							try { num = int.Parse(choise); }
							catch (FormatException) { Console.WriteLine(" Ошибка ввода номера.", ConsoleColor.DarkRed); continue; }

							int idProduct;
							try { idProduct = numId[num - 1]; }
							catch (ArgumentOutOfRangeException) { Console.WriteLine(" Ошибка ввода номера.", ConsoleColor.DarkRed); continue; }
							Product selectProduct = null;
							groupProducts.ForEach(g => {
								var prod = g.FirstOrDefault(p => p.Id == idProduct);
								if (prod != null)
								{
									selectProduct = prod;
									if(selectOrganization == null)
										selectOrganization = g.Key;
									
									return;
								}
							});
							if(selectProduct == null)
                            {
								Console.WriteLine("Ошибка ввода номера товара.");
								continue;
                            }
							if (selectOrganization.ListSeller.Count() == 0)
								Console.WriteLine("К сожалению данный продукт пока что не может быть заказан, попробуйте позже.");
							else
							{
								basket.Add(selectProduct);
								Console.WriteLine($"{selectProduct} - добавлен");
							}
						}

						var linkCW = customer.BuyProduct(basket, selectOrganization.ListSeller.First());
						if (linkCW != null)
							foreach(var l in linkCW)
							{
								Console.WriteLine($"{l.Item1} доставит товары:");
								l.Item2.ListProduct.ToList().ForEach(p=>Console.WriteLine($" {p}"));
							}
                        else
                        {
							Console.WriteLine("К сожалению доставка сейчас невозможна, попробуйте позже.");
						}

						//customer.BuyProduct(products);
						break;
					case (exit):
						codeExit = true;
						break;
				}
				if (codeExit) break; 
				Console.WriteLine();
			}
			Console.ForegroundColor = defaultColor;
			Console.WriteLine();
		}

		private void MakeWorkSeller(Seller seller)
		{
			var defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			const char community = 'B';
			const char exit = 'E';
			while (true)
			{
				bool codeExit = false;
				var buttons = new Dictionary<char, string>() 
				{ 
					{ community, "Найти товар на складах" },
					{ exit, "Выйти из профиля продавца" } 
				};
				var choice = GetPromptChoice(buttons);
				
				switch (choice)
				{
					case (community)://узнать о наличии товара на складах
						List<Product> products = FindProductByName();
						if (products == null || products.Count() == 0)
						{
							Console.WriteLine($"Товар не найден.");
							break;
						}
						var prodWareHosuse = seller.GetWarehouseHaving(products);
						if (prodWareHosuse.Count() == 0)
						{
							Console.WriteLine($"Продукты не найдены на складах");
							products.ForEach(p => Console.WriteLine($" \"{p}\" не найден "));
						}
						else
						{
							foreach (var pw in prodWareHosuse)
							{
								Console.WriteLine($"Продукт \"{pw}\" найден:");
								Console.WriteLine($" на складе {pw.WareHouse.Address}", ConsoleColor.DarkGreen);
							}
						}
						break;
					case (exit):
						codeExit = true;
						break;

				}
				if (codeExit) break;
				Console.WriteLine();
			}
			Console.ForegroundColor = defaultColor;
			Console.WriteLine();
		}

		private void MakeWorkCourier(Courier courier)
		{
			var defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			const char activity = 'A';
			const char passivity = 'P';
			const char exit = 'E';

			while (true)
			{
				bool codeExit = false;
				string prefix = null;
				var buttons = new Dictionary<char, string>()
				{
					{ activity, "стать активным курьером" },
					{ passivity, "стать пассивным курьером" },
					{ exit, "выйти из профиля клиента" }
				};
				var choice = GetPromptChoice(buttons);

				switch (choice)
				{
					case (activity)://стать активным курьером
						courier.MakeActivity(true);
						courier.UpdateInDB();
						break;

					case (passivity)://стать пассивным курьером
						courier.MakeActivity(false);
						courier.UpdateInDB();
						prefix = "не";
						break;

					case (exit):
						codeExit = true;
						break;
				}
				Console.WriteLine($"{courier} - стал {prefix} активным.");
				if (codeExit) break;
				Console.WriteLine();
			}
			Console.ForegroundColor = defaultColor;
			Console.WriteLine();
		}

		private void MakeWorkDispatcher(Dispatcher dispatcher)
		{
			var defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			const char community = 'C';
			const char exit = 'E';
			while (true)
			{
				bool codeExit = false;
				var buttons = new Dictionary<char, string>()
				{
					{ community, "связаться с продацом и узнать о наличии товара" },
					{ exit, "выйти из профиля клиента" }
				};
				var choice = GetPromptChoice(buttons);
				switch (choice)
				{
					case (community)://связаться с продацом и узнать о наличии товара
						List<Product> products = FindProductByName();
						if (products == null || products.Count() == 0)
						{
							Console.WriteLine($"Продукт не найден.");
							break;
						}
						List<Seller> seller = dispatcher
								.GetSellerHavingProducts(products)
								.ToList();
						if (seller == null || seller.Count() == 0)
						{
							Console.WriteLine("Ни один продавец не имеет в наличии:");
							products.ForEach(p => Console.WriteLine($" {p}"));
						}
						else
							seller.ForEach(s => Console.WriteLine($" Организация: {s.Organization.Name} продавец: {s.Name} (тел. {s.NumberPhone})"));
						break;

					case (exit):
						codeExit = true;
						break;
				}
				if (codeExit) break;
				Console.WriteLine();
			}
			Console.ForegroundColor = defaultColor;
			Console.WriteLine();
		}

		private void MakeWorkAdmin()
        {
			var defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Gray;
			const char add = 'A';
			const char exit = 'E';
            while (true)
			{
				bool codeExit = false;
				var buttons = new Dictionary<char, string>()
				{
					{ add, "добавить нового пользователя" },
					{ exit, "выйти из профиля клиента" }
				};
				var choice = GetPromptChoice(buttons);
				switch (choice)
                {
					case (add):
						Console.WriteLine("Что добавить?");
						var type1Types =
								from type in Assembly.GetExecutingAssembly().GetTypes()
								where type.IsDefined(typeof(TableAttribute), false)
								select type;
						foreach (var t in type1Types.Select(x => x.Name))
							Console.WriteLine($" {t}");
						Console.ReadLine();
						break;
					case (exit):
						codeExit = true;
						break;
				}
				if (codeExit) break;
				Console.WriteLine();
			}
			Console.ForegroundColor = defaultColor;
			Console.WriteLine();

		}

		private List<Product> FindProductByName()
        {
			Console.Write("Какой продукт вы хотите: ");	
			var productName = Console.ReadLine().ToLower().Trim();
			return Product.FindInDBByName(productName);
		}

		private char GetPromptChoice(Dictionary<char,string> buttons)
		{
			foreach( var button in buttons)
				Console.WriteLine($"{button.Key} - {button.Value}");

			var key = Console.ReadKey(true);
			return char.ToUpper(key.KeyChar);
		}
	}
}
