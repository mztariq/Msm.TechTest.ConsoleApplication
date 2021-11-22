using Msm.TechTest.Domain;
using Msm.TechTest.Domain.DiscountOffers;
using System;
using System.Collections.Generic;
using Xunit;

namespace MsmTechTest.Tests.Unit
{
    public class ButerBreadDiscountTests
    {
        [Fact]
        public void Given_Null_Products_Then_Throw_Argument_Null_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new ButterBreadDiscount().GetDiscount(null));
        }

        [Fact]
        public void Given_Three_butter_No_Bread_Then_Return_No_Discount()
        {
            // ARRANGE
            var _products = new List<Product>();
            _products.Add(new Product { Name = "butter", Cost = 1.00m });
            _products.Add(new Product { Name = "butter", Cost = 1.00m });
            _products.Add(new Product { Name = "butter", Cost = 1.00m });

            // ACT
            var result = new ButterBreadDiscount().GetDiscount(_products);

            // ASSERT
            Assert.Equal(0, result);
        }

        [Fact]
        public void Given_Three_Butter_One_Bread_Return_0_5_Discount()
        {
            // ARRANGE
            var items = GenerateProductCombo();

            // ACT
            var result = new ButterBreadDiscount().GetDiscount(items);

            // ASSERT
            Assert.Equal(0.4m, result);
        }

        private static IList<Product> GenerateProductCombo()
        {
            var products = new List<Product>();
            products.Add(new Product { Name = "butter", Cost = 1.00m });
            products.Add(new Product { Name = "butter", Cost = 1.00m });
            products.Add(new Product { Name = "butter", Cost = 1.00m });
            products.Add(new Product { Name = "bread", Cost = 0.80m });
            return products;
        }
    }
}
