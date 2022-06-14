using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModulDelivery.Domain.Models;

namespace ModulDelivery.Infrastructure
{
    /// <summary>
    /// Класс для дополнительных методов работы с БД.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Найти организацию в БД по наименованию
        /// </summary>
        /// <param name="organization">Искомая организация</param>
        /// <returns>true, если организация нашлась в БД, false - не нашлась</returns>
        public static Organization FindInDB(Organization organization)
        {
            Organization havingOrg;
            using (var db = new DeliveryContext())
            {
                havingOrg = db.Organization
                    .Where(org => org.Name == organization.Name)
                    .FirstOrDefault();
            }
            return havingOrg;
        }
    }
}
