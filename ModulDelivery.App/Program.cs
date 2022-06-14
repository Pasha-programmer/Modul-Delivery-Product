using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ModulDelivery.Domain.Models;
using ModulDelivery.Infrastructure;//

namespace ModulDelivery.App
{
    class Program
    {
        static void Main(string[] args)
        {
            new DeliveryConsoleUi().Run();
        }
    }
}
