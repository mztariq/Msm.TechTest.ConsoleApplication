using System.Threading.Tasks;

namespace Msm.TechTest.Application.Services
{
    public interface IBasketPriceCalculator
    {
        void Scan(string productName);
        Task<decimal> GetTotalPrice();
    }
}
