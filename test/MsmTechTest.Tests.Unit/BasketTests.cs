using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MsmTechTest.Tests.Unit
{
    public class BasketTests
    {
        private readonly IBasketPriceCalculator _basketPriceCalculator;
        private readonly object _products = new();

        public BasketTests()
        {
            var ItemList = new[]
            {
                new Product{Name = "butter", Cost = 0.80m},
                new Product{Name = "milk", Cost = 1.15m},
                new Product{Name = "bread", Cost = 1.00m}
            };

            var discounts = new[]
            {
                new Offer{Name = "butter", Quantity = 2, Cost = 0.80m},
                new Offer{Name= "milk", Quantity = 4, Cost = 3.45m}
            };

            _basketPriceCalculator = new BasketPriceCalculator(ItemList, discounts);
            _products = new();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void When_No_Items_Scanned_Return_Zero(string items)
        {
            var expectedPrice = 0;
            _basketPriceCalculator.Scan(items);
            var actualPrice = _basketPriceCalculator.GetTotalPrice(null).Result;

            Assert.Equal(expectedPrice, actualPrice);
        }




        public static IEnumerable<object[]> SingleItemsData =>
        new List<object[]>
        {
            new object[] { "Butter", 0.80m },
            new object[] { "Milk", 1.15m },
            new object[]{ "Bread", 1.00m }
        };

        [Theory]
        [MemberData(nameof(SingleItemsData))]
        public void When_Single_Item_Scanned_Return_Correct_Price(string productName, decimal productPrice)
        {
            var expectedPrice = productPrice;
            _basketPriceCalculator.Scan(productName);
            var actualPrice = _basketPriceCalculator.GetTotalPrice(null).Result;
            Assert.Equal(expectedPrice, actualPrice);
        }


        [Theory]
        [InlineData("Butter,Milk,Bread", 2.95)]
        [InlineData("BUTTEr,mILK,breaD", 2.95)]
        public void When_Items_Without_Discount_Scanned_Return_Correct_Price(string skuCode, decimal skuPrice)
        {
            var expectedPrice = skuPrice;
            _basketPriceCalculator.Scan(skuCode);
            var actualPrice = _basketPriceCalculator.GetTotalPrice(null).Result;

            Assert.Equal(expectedPrice, actualPrice);
        }


        [Theory]
        [InlineData("Butter,Butter,Bread,Bread", 3.10)]
        public void When_Items_Without_Scanned_Return_Correct_Discounted_Price_With_Butter_Bread_Offer(string skuCode, decimal skuPrice)
        {
            var expectedPrice = skuPrice;
            _basketPriceCalculator.Scan(skuCode);
            var actualPrice = _basketPriceCalculator.GetTotalPrice("bread").Result;

            Assert.Equal(expectedPrice, actualPrice);
        }

        [Theory]
        [InlineData("Milk,milk,MiLk,MILK", 3.45)]
        public void When_Items_Without_Scanned_Return_Correct_Discounted_Price_With_Multiple_Milk(string skuCode, decimal skuPrice)
        {
            var expectedPrice = skuPrice;
            _basketPriceCalculator.Scan(skuCode);
            var actualPrice = _basketPriceCalculator.GetTotalPrice("milk").Result;

            Assert.Equal(expectedPrice, actualPrice);
        }

    }

    internal interface IBasketPriceCalculator
    {
        void Scan(string productName);
        Task<decimal> GetTotalPrice(string offer);
    }

    internal class BasketPriceCalculator : IBasketPriceCalculator
    {
        private readonly IList<Product> _products;
        private readonly IList<Offer> _discounts;
        private List<string> scannedProducts;

        public BasketPriceCalculator(IList<Product> products, IList<Offer> discounts)
        {
            _products = products;
            _discounts = discounts;
            scannedProducts = new List<string>();
        }

        public async Task<decimal> GetTotalPrice(string offer)
        {
            if (scannedProducts.Count == 0)
            { return 0; }
            decimal total = 0;
            decimal totalDiscount = 0;
            List<Product> productList = new();
            foreach (var product in scannedProducts)
            {
                var cost = _products.Single(s => s.Name == product).Cost;
                total += cost;
                productList.Add(new Product { Name = product, Cost = cost });
            }

            IDiscountCalculator discountCalculator = offer == "bread" ? new ButterBreadDiscount() : new MilkDiscount();

            totalDiscount = discountCalculator.GetDiscount(productList);

            return total - totalDiscount;
        }

        public void Scan(string items)
        {
            if (!string.IsNullOrEmpty(items))
            {
                foreach (var product in items.Split(','))
                {
                    scannedProducts.Add(product.ToLower());
                }
            }
            else
            {
                scannedProducts = new List<string>();
            }
        }

        //private decimal CalculateDiscount(List<string> products)
        //{
        //    IDiscountCalculator discountCalculator = new ButterBreadDiscount();
        //    var discountTotal = 0.00m;
        //    foreach (var product in products)
        //    {
        //        discountTotal -= discountCalculator.GetDiscount(products);
        //    }
        //    return discountTotal;
        //    //return (int)(itemCount / discount.Quantity * discount.Cost);
        //}

    }



    //============= Discount Calculators using strategy pattern ================


    public interface IDiscountCalculator
    {
        decimal GetDiscount(IList<Product> scannedProducts);
    }


    public class ButterBreadDiscount : IDiscountCalculator
    {

        public decimal GetDiscount(IList<Product> scannedProducts)
        {
            var breadCount = scannedProducts.Where(x => x.Name == "bread").Count();
            var butterCount = scannedProducts.Where(x => x.Name == "butter").Count();
            var breadPrice = scannedProducts.FirstOrDefault(x => x.Name == "bread")?.Cost; // scannedProducts.Select(x => x.Cost).Where(z => z. == "bread");

            if (butterCount >= 2)
            {
                return (decimal)(Math.Ceiling(((decimal)breadCount) / 2) * (breadPrice / 2));
            }

            return 0;
        }

    }

    public class MilkDiscount : IDiscountCalculator
    {
        public decimal GetDiscount(IList<Product> scannedProducts)
        {

            var milkCount = scannedProducts.Where(x => x.Name == "milk").Count();
            var milkPrice = scannedProducts.FirstOrDefault(x => x.Name == "milk")?.Cost;

            // Apply offer is more than 3 milks.
            if (milkCount > 3)
            {
                return (decimal)(Math.Floor((decimal)milkCount / 3) * milkPrice);
            }

            return 0;
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }
    }

    public class Offer
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
    }
}
