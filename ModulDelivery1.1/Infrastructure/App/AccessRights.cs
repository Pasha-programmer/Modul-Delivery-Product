using System;
using System.Collections.Generic;
using System.Text;

namespace ModulDelivery.Infrastructure
{
    public enum AccessRights
    {
        //[Label("Администратор")]
        //Admin,
        [Label("Курьер")]
        Courier,
        [Label("Клиент")]
        Customer,
        [Label("Продавец")]
        Seller,
        [Label("Диспетчер")]
        Dispatcher
    }
}
