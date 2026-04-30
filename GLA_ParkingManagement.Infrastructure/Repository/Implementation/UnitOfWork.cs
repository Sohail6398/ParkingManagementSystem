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
    public class UnitOfWork: IUnitOfWork, IDisposable
    {
        private readonly ParkingManagementDbContext _context;
        private Hashtable _repositories;
        private bool _disposed = false;
        public UnitOfWork(ParkingManagementDbContext context)
        {
            _context = context;
        }
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_context != null)   // ✅ FIX
                    {
                        _context.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        public async Task<int> SaveAsync()
        {
            var data = await _context.SaveChangesAsync();
            return data;
        }

    }
}
