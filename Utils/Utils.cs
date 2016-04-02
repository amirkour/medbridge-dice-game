using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    /// <summary>
    /// Just a bunch of utility/helper functions
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// Various helpers that true if the given collections are null or empty, false otherwise
        /// </summary>
        public static bool IsNullOrEmpty<T>(this List<T> list) { return list == null || list.Count <= 0; }
        public static bool IsNullOrEmpty(this object[] arr) { return arr == null || arr.Length <= 0; }
        public static bool IsNullOrEmpty(this string str) { return str == null || str.Length <= 0; }
    }
}
