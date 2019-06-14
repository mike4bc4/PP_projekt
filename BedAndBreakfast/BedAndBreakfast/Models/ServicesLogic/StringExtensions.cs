using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public static class StringExtensions
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

        public static string FirstLettersToUpperCase(string str) {
            List<string> strWords = str.Trim().Split(' ').ToList();
            var sb = new StringBuilder();
            foreach (string word in strWords) {
                sb.Append(word.First().ToString().ToUpper() + word.Substring(1));
                sb.Append(" ");
            }
            return sb.ToString().TrimEnd();          
        }



    }
}
