using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public static class StringManager
    {
        public static string RemoveSpecials(string str) {
            var sb = new StringBuilder();
            foreach (char c in str) {
                if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == ' ' || c >= '0' && c <= '9') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }



    }
}
