using Msm.TechTest.Domain;
using Msm.TechTest.Domain.DiscountOffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Msm.TechTest.Application.Services
{
    public class BasketPriceCalculator : IBasketPriceCalculator
    {
        private readonly IList<Product> _products;
        private readonly IList<IDiscountCalculator> _discounts;
        private List<string> scannedProducts;

        public BasketPriceCalculator(IList<Product> products, IList<IDiscountCalculator> discounts)
        {
            _products = products;
            _discounts = discounts;
            scannedProducts = new List<string>();
        }

        public async Task<decimal> GetTotalPrice()
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

            if (_discounts != null)
            {
                foreach (var discount in _discounts)
                {
                    totalDiscount += discount.GetDiscount(productList);
                }
            }

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
    }
}
