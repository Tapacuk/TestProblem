using System;
using System.Collections.Generic;
using System.Text;

namespace TestProblem
{
    public class ProductINFO
    {
        public static List<ProductINFO> Products = new List<ProductINFO>(); // contains of all bought products
        public static Dictionary<int, double> ProfitPerYear = new Dictionary<int, double>(); // int - year, double - earnings during the year
        public DateTime Date   { get; set; }
        public double Price    { get; set; }
        public string Currency { get; set; }
        public string Name     { get; set; }
    }
}
