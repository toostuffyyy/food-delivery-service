using desktop.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace desktop.Services
{
    public interface IStatisticRepository
    {
        [Get("/Statistic/GetProductStatistics")]
        public Task<IEnumerable<ProductStatistic>> GetProductStatistics([Authorize("Bearer")] string accessToken, [Query] DateRange dateRange);
    }
}
