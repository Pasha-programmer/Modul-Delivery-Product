using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModulDelivery.Infrastructure;

namespace ModulDelivery.Domain.Models
{
    internal class Delivery
    {
        public Tuple<Warehouse, TimeSpan> GetTimeDelivery(List<Product> products, Seller seller, Customer customer)
        {
            var allWarehouse = seller.GetWarehouseHaving(products);
            Warehouse nearestWarehouse = null;
            int distance = allWarehouse.Min(x => {
                nearestWarehouse = x.WareHouse;
                return x.WareHouse.Address.GetDistanse(customer.Address);
                }
            );

            //TODO: реализовать рассчет времени досатвки

            int velocity = 80;//km/h
            return new Tuple<Warehouse, TimeSpan>(nearestWarehouse, new TimeSpan(distance / velocity));//прмиерное время доставки
        }

        public Dispatcher GetDispatcher()
        {
            Dispatcher dispetcher;
            using (var db = new DeliveryContext())
            {
                dispetcher = db.Dispatcher
                    .Where(d => d.IsActivity)
                    .FirstOrDefault();
            }
            return dispetcher;
        }

        
    }
}
