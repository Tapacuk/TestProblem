using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;

namespace TestProblem
{
    public class Executer
    {
        public void ExecuteCommand(string[] cmd, ProductINFO product = null)
        {
            if (Equals(cmd[0], "all"))
            {
                ShowProducts();
            }

            else if (Equals(cmd[0], "clear"))
            {
                if(ProductINFO.Products.Count() != 0)
                    ProductINFO.Products.RemoveAll(x => string.Equals(x.Date.ToString("dd-MM-yyyy"), Convert.ToDateTime(cmd[1]).ToString("dd-MM-yyyy")));
                ShowProducts();
            }

            else if (Equals(cmd[0], "purchase"))
            {
                double price;
                DateTime date;
                DateTime.TryParseExact(cmd[1], "dd-MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date);             // cmd[1] == date
                Double.TryParse(cmd[2], out price);         // cmd[2] == Price
                product.Currency = cmd[3];                  // cmd[3] == currency
                product.Name     = cmd[4];                  // cmd[4] == name
                product.Price    = price;
                product.Date     = date;

                ProductINFO.Products.Add(product);
                ShowProducts();
            }

            else if (Equals(cmd[0], "report"))
            {
                int year = Int32.Parse(cmd[1]);
                ProductINFO.ProfitPerYear[year] = 0;
                if (ProductINFO.Products.Count() != 0)
                {
                    Fixer fixer = new Fixer();
                    string str = string.Empty;
                    var prods = from p in ProductINFO.Products
                                where p.Date.Year == year
                                select p; // taking products bought during input year
                    foreach (ProductINFO p in prods)
                    {
                        if (string.Equals(p.Currency, cmd[2]))
                            ProductINFO.ProfitPerYear[year] += p.Price; // adding products with input currency
                        else
                        {
                            str = fixer.FormString(p.Price, p.Currency, cmd[2]);
                            ProductINFO.ProfitPerYear[year] += fixer.GetPrice(str, p.Price, cmd[2]); // converting price to input currency
                        }
                    }
                }
                Console.WriteLine($"Earnings in {cmd[1]}: {ProductINFO.ProfitPerYear[year]} {cmd[2]}");
            }
        }

        void ShowProducts()
        {
            var dateGroups = from pr in ProductINFO.Products
                             group pr by pr.Date; // dividing products into groups (depending on year)
            if (dateGroups.Count() == 0) Console.WriteLine("No products have been bought");
            else
            {
                Console.WriteLine("###############################");
                foreach (IGrouping<DateTime, ProductINFO> g in dateGroups)
                {
                    Console.WriteLine(g.Key.ToString("dd-MM-yyyy"));
                    foreach (var t in g)
                    {
                        Console.WriteLine($"{t.Name} {t.Price} {t.Currency}");
                    }
                    Console.WriteLine("###############################");
                }
            }
            
        }

        public bool CheckParams(string[] str)
        {
            if (str.Length > 5 || str.Length == 4) return false; // too less or not enough arguments
            for (int i = 0; i < str.Length; i++) // checking each argument
            {
                switch (i)
                {
                    case 0:
                    {
                        if (!string.Equals(str[i], "purchase") && !string.Equals(str[i], "all")
                            && !string.Equals(str[i], "clear") && !string.Equals(str[i], "report"))
                            return false; // no correct command
                        if (string.Equals(str[i], "purchase") && str.Length != 5) return false;   // amount of necessary
                        else if (string.Equals(str[i], "all") && str.Length != 1) return false;   // parameters depending
                        else if (string.Equals(str[i], "clear") && str.Length != 2) return false; // on command
                        else if (string.Equals(str[i], "report") && str.Length != 3) return false;
                        break;
                    }
                    case 1:
                    {
                        DateTime temp;
                        if (string.Equals(str[0], "report") && DateTime.TryParseExact(str[1], "yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out temp))
                            break;        // [CMD] report requires 3 parameters
                        if (!DateTime.TryParse(str[i], out temp))
                            return false; // [CMD] clear requires just  2 parameters
                        break;
                    }
                    case 2:
                    {
                        double temp;
                        if (Double.TryParse(str[i], out temp) && !string.Equals(str[0], "report"))
                            break;
                        else if (IsCurrency(str[i]))
                            return true;  // end for [CMD] report
                        return false;
                    }
                    case 3:
                    {
                        if (!IsCurrency(str[i]))
                            return false; // verifying for purchasing [CMD]
                        break;
                    }
                    case 4:
                    {
                        if (str[i].Length > 15 || IsDigitsOnly(str[i])) return false;
                        break;            // verifying the name
                    }
                }
            }

            return true;
        }

        bool IsCurrency(string str)
        {
            if (!Equals(str, "USD") && !Equals(str, "EUR")
               && !Equals(str, "UAH") && !Equals(str, "PLN")) // USD not supported in free version
                return false;
            return true;
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            } // checks whether name contains only from digits
            return true;
        }

        public static void Menu()
        {
            string command;    // command to write
            string[] words;    // command splitted into separated instructions
            Executer executer = new Executer();
            ProductINFO product = null;
            bool stop = false; // indicates the stop of program
            string Answer;     // "Yes" to stop the program | "No" to continue
            do
            {
                Console.WriteLine("Enter a command to execute: ");
                Console.Write("-> ");
                command = Console.ReadLine();
                words = command.Split(' ');
                if (words.Length == 5) product = new ProductINFO(); // [CMD] purchase requires 5 arguments
                if (!executer.CheckParams(words)) // checking the parameters
                {
                    Console.WriteLine("[Error] Uncorrect command");
                    continue;
                }
                executer.ExecuteCommand(words, product); // Execute the command
                Console.WriteLine("Would you like to enter one more command?(Print: Yes or No)");
                Console.Write("-> ");
                Answer = Console.ReadLine();
                while (!string.Equals(Answer, "Yes") && !string.Equals(Answer, "No")) // Uncorrect answer
                {
                    Console.WriteLine("Enter correct answer:");
                    Console.Write("-> ");
                    Answer = Console.ReadLine();
                }
                if (string.Equals(Answer, "Yes")) stop = false;
                else if (string.Equals(Answer, "No")) stop = true;
            } while (!stop);
        }
    }
}
