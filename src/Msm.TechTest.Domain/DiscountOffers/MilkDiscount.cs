using Msm.TechTest.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Msm.TechTest.Domain.DiscountOffers
{
    public class MilkDiscount : IDiscountCalculator
    {
        public decimal GetDiscount(IList<Product> scannedProducts)
        {
            if(scannedProducts == null)
            {
                throw new ArgumentNullException();
            }
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
}
