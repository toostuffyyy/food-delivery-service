using System.Collections.Generic;
using System.Threading.Tasks;
using desktop.Models;
using Refit;

namespace desktop.Service;

public interface IStatusRepository
{
    [Get("/Status/GetStatus")]
    public Task<IEnumerable<Status>> GetStatus();
}