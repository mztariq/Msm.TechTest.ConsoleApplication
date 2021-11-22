using Msm.TechTest.Application.Services;
using Msm.TechTest.Domain;
using Msm.TechTest.Domain.DiscountOffers;
using System.Collections.Generic;
using Xunit;

namespace MsmTechTest.Tests.Unit
{
    public class BasketTests
    {
        private readonly IList<Product> _products;

        public BasketTests()
        {
            _products = new[]
            {
                new Product{Name = "butter", Cost = 0.80m},
                new Product{Name = "milk", Cost = 1.15m},
                new Product{Name = "bread", Cost = 1.00m}
            };
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void When_No_Items_Scanned_Return_Zero(string items)
        {
            var expectedPrice = 0;
            var basketPriceCalculator = new BasketPriceCalculator(_products, null);

            basketPriceCalculator.Scan(items);
            var actualPrice = basketPriceCalculator.GetTotalPrice().Result;

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
            var basketPriceCalculator = new BasketPriceCalculator(_products, null);

            basketPriceCalculator.Scan(productName);
            var actualPrice = basketPriceCalculator.GetTotalPrice().Result;

            Assert.Equal(expectedPrice, actualPrice);
        }


        [Theory]
        [InlineData("Butter,Milk,Bread", 2.95)]
        [InlineData("BUTTEr,mILK,breaD", 2.95)]
        public void When_Items_Without_Discount_Scanned_Return_Correct_Price(string skuCode, decimal skuPrice)
        {
            var expectedPrice = skuPrice;
            var basketPriceCalculator = new BasketPriceCalculator(_products, null);

            basketPriceCalculator.Scan(skuCode);
            var actualPrice = basketPriceCalculator.GetTotalPrice().Result;

            Assert.Equal(expectedPrice, actualPrice);
        }


        [Theory]
        [InlineData("Butter,Butter,Bread,Bread", 3.10)]
        public void When_Items_Without_Scanned_Return_Correct_Discounted_Price_With_Butter_Bread_Offer(string skuCode, decimal skuPrice)
        {
            var expectedPrice = skuPrice;
            var discounts = new List<IDiscountCalculator>
            {
                new ButterBreadDiscount()
            };
            var basketPriceCalculator = new BasketPriceCalculator(_products, discounts);

            basketPriceCalculator.Scan(skuCode);
            var actualPrice = basketPriceCalculator.GetTotalPrice().Result;

            Assert.Equal(expectedPrice, actualPrice);
        }

        [Theory]
        [InlineData("Milk,milk,MiLk,MILK", 3.45)]
        public void When_Items_Without_Scanned_Return_Correct_Discounted_Price_With_Multiple_Milk(string skuCode, decimal skuPrice)
        {
            var expectedPrice = skuPrice;
            var discounts = new List<IDiscountCalculator>
            {
                new MilkDiscount()
            };
            var basketPriceCalculator = new BasketPriceCalculator(_products, discounts);

            basketPriceCalculator.Scan(skuCode);
            var actualPrice = basketPriceCalculator.GetTotalPrice().Result;

            Assert.Equal(expectedPrice, actualPrice);
        }

        [Theory]
        [InlineData("butter,butter,bread,Milk,milk,MiLk,MILK,Milk,milk,MiLk,MILK", 9.00)]
        public void When_Items_Without_Scanned_Return_Correct_Discounted_Price_With_Milk_And_ButterBread(string skuCode, decimal skuPrice)
        {
            var expectedPrice = skuPrice;
            var discounts = new List<IDiscountCalculator>
            {
                new ButterBreadDiscount(),
                new MilkDiscount()
            };
            var basketPriceCalculator = new BasketPriceCalculator(_products, discounts);

            basketPriceCalculator.Scan(skuCode);
            var actualPrice = basketPriceCalculator.GetTotalPrice().Result;

            Assert.Equal(expectedPrice, actualPrice);
        }

        [Theory]
        [InlineData("Milk,milk,MiLk,MILK,Milk,milk,MiLk,MILK", 6.90)]
        public void When_Items_Without_Scanned_Return_Correct_Discounted_Price_With_All_Milk(string skuCode, decimal skuPrice)
        {
            var expectedPrice = skuPrice;
            var discounts = new List<IDiscountCalculator>
            {
                new ButterBreadDiscount(),
                new MilkDiscount()
            };
            var basketPriceCalculator = new BasketPriceCalculator(_products, discounts);

            basketPriceCalculator.Scan(skuCode);
            var actualPrice = basketPriceCalculator.GetTotalPrice().Result;

            Assert.Equal(expectedPrice, actualPrice);
        }

    }
}
