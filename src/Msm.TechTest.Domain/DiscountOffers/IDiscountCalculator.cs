using Msm.TechTest.Domain;
using System.Collections.Generic;

namespace Msm.TechTest.Domain.DiscountOffers
{
    public interface IDiscountCalculator
    {
        decimal GetDiscount(IList<Product> scannedProducts);
    }
}
