using desktop.Models;
using System.Threading.Tasks;

namespace desktop.Services
{
    public interface IReportProductService
    {
        public Task SaveDocument(ProductStatistic[] productStatistics, DateRange dateRange);
    }
}
