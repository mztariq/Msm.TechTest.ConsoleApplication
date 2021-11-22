using Msm.TechTest.Domain;
using Msm.TechTest.Domain.DiscountOffers;
using System;
using System.Collections.Generic;
using Xunit;

namespace MsmTechTest.Tests.Unit
{
    public class MilkDiscountTests
    {
        [Fact]
        public void Given_Null_Products_Then_Throw_Argument_Null_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new MilkDiscount().GetDiscount(null));
        }


        [Theory]
        [InlineData("milk")]
        [InlineData("MILK")]
        public void Given_One_Milk_Then_Return_No_Discount(string name)
        {
            // ARRANGE
            var _products = new List<Product>();
            _products.Add(new Product { Name = name, Cost = 1.15m });

            // ACT
            var result = new MilkDiscount().GetDiscount(_products);

            // ASSERT
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(8, 2.3)]
        [InlineData(4, 1.15)]
        [InlineData(16, 5.75)]
        public void Given_Multiply_Milk_Then_Return_Correct_Discount(int milkAmount, decimal expected)
        {
            // ARRANGE
            var items = GenerateMilk(milkAmount);

            // ACT
            var result = new MilkDiscount().GetDiscount(items);

            // ASSERT
            Assert.Equal(expected, result);
        }

        private static IList<Product> GenerateMilk(int amount)
        {
            var milks = new List<Product>();

            for (int i = 0; i < amount; i++)
            {
                milks.Add(new Product { Name = "milk", Cost = 1.15m });
            }

            return milks;
        }
    }
}
