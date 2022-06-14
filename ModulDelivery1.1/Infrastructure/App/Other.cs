using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ModulDelivery.Infrastructure
{
    public static class Other
    {
        public static string CheckNumberPhone(string number)
        {
            string pattern = @"\D";
            string target = "";
            Regex regex = new Regex(pattern);
            return number == null ? null : regex.Replace(number, target);
        }
    }
}
