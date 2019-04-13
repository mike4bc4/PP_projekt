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

        public static string RemoveDuplicateWords(string str) {
            List<string> words = str.Trim().Split(' ').ToList();
            
            for (int i = 0; i < words.Count(); i++) {
                for (int j = 0; j < words.Count(); j++) {
                    if (i != j && words[i] == words[j])
                    {
                        words[i] = string.Empty;
                    }
                }

            }

            string output = string.Empty;
            for (int i = 0; i < words.Count(); i++)
            {
                if (string.IsNullOrEmpty(words[i])) {
                    words.RemoveAt(i);
                }
                output += (words[i] + " ");
            }
            return output.TrimEnd();
        }


    }
}
