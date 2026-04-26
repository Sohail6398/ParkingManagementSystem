using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Infrastructure.Repository.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        Task<int> SaveAsync();
    }
}
