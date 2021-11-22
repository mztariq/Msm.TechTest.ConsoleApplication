using Msm.TechTest.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Msm.TechTest.Domain.DiscountOffers
{
    public class ButterBreadDiscount : IDiscountCalculator
    {
        public decimal GetDiscount(IList<Product> scannedProducts)
        {
            if (scannedProducts == null)
            {
                throw new ArgumentNullException();
            }
            var breadCount = scannedProducts.Where(x => x.Name == "bread").Count();
            var butterCount = scannedProducts.Where(x => x.Name == "butter").Count();
            var breadPrice = scannedProducts.FirstOrDefault(x => x.Name == "bread")?.Cost;

            if (breadPrice == null)
            { return 0; }

            if (butterCount >= 2)
            {
                return (decimal)(Math.Ceiling((decimal)breadCount / 2) * (breadPrice / 2));
            }

            return 0;
        }

    }
}
