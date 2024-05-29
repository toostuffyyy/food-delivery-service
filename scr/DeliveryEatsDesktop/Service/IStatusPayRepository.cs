using System.Collections.Generic;
using System.Threading.Tasks;
using desktop.Models;
using Refit;

namespace desktop.Service;

public interface IStatusPayRepository
{
    [Get("/StatusPay/GetStatusPay")]
    public Task<IEnumerable<StatusPay>> GetStatusPay();
}