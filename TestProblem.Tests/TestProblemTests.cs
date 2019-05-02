using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace TestProblem.Tests
{
    [TestClass]
    public class TestProblemTests
    {
        [TestMethod]
        public void ExcecuteCommandClear_Test()
        {
            //set
            ProductINFO.Products = new System.Collections.Generic.List<ProductINFO> { 
                { new ProductINFO { Date = Convert.ToDateTime("01.01.2019"), Price = 50.0, Currency = "USD", Name = "Beer" }},
                { new ProductINFO { Date = Convert.ToDateTime("01.01.2018"), Price = 50.0, Currency = "USD", Name = "Beer" }}
            };
            int expectedCount = ProductINFO.Products.Count-1; // decreasing of products count
            string command = "clear 2019-01-01";
            string[] words = command.Split(' ');

            //act
            Executer executer = new Executer();
            executer.ExecuteCommand(words);
            int actualCount = ProductINFO.Products.Count;

            //assert
            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod]
        public void ExcecuteCommandPurchase_Test()
        {
            //set
            ProductINFO.Products = new System.Collections.Generic.List<ProductINFO>();
            DateTime date;
            DateTime.TryParseExact("2019-01-01", "dd-MM-yyyy", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date);
            ProductINFO product = new ProductINFO
            { Date = date, Price = 50.0, Currency = "EUR", Name = "Beer" };
            string command = "purchase 2019-01-01 50 EUR Beer";
            string[] words = command.Split(' ');
            
            //act
            Executer executer = new Executer();
            executer.ExecuteCommand(words, new ProductINFO());

            //assert
            Assert.AreEqual(product.Date, ProductINFO.Products[0].Date);
            Assert.AreEqual(product.Price, ProductINFO.Products[0].Price);
            Assert.AreEqual(product.Currency, ProductINFO.Products[0].Currency);
            Assert.AreEqual(product.Name, ProductINFO.Products[0].Name);
        }

        [TestMethod]
        public void ExcecuteCommandReport_Test()
        {
            //set
            ProductINFO.Products = new System.Collections.Generic.List<ProductINFO> {
                { new ProductINFO { Date = Convert.ToDateTime("01.01.2019"), Price = 50.0, Currency = "EUR", Name = "Beer" }},
                { new ProductINFO { Date = Convert.ToDateTime("01.02.2019"), Price = 50.0, Currency = "UAH", Name = "Beer" }}
            };
            double expectedValue = 1527.0641; // Change Value depening on rates
            string command = "report 2019 UAH";
            string[] words = command.Split(' ');

            //act
            Executer executer = new Executer();
            executer.ExecuteCommand(words);

            //assert
            Assert.AreEqual(expectedValue, ProductINFO.ProfitPerYear[Int32.Parse(words[1])]);
        }

        [TestMethod]
        public void CheckParams_Test()
        {
            //set
            string[][] commands = new string[3][];
            commands[0] = new string[] {};
            commands[1] = new string[] {};
            commands[2] = new string[] {};
            bool[] expectedValue = new bool[3] { false, false, false };

            commands[0] = "clear 2019".Split(' ');
            commands[1] = "purchase 2019-01-01 50 UAH".Split(' ');
            commands[2] = "report 2019-01-01 50 USD Beer".Split(' ');
            
            //act
            Executer executer = new Executer();

            //assert
            for (int i = 0; i < 3; i++)
                Assert.AreEqual(expectedValue[i], executer.CheckParams(commands[i]));
            
        }
    }
}
