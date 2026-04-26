using GLA_ParkingManagement.Infrastructure.Database;
using GLA_ParkingManagement.Infrastructure.Repository.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Infrastructure.Repository.Implementation
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ParkingManagementDbContext _context;
        private Hashtable _repositories;
        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }
            var type = typeof(T).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<T>(_context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IGenericRepository<T>)_repositories[type]!;
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            var data = await _context.SaveChangesAsync();
            return data;
        }
    }
}
