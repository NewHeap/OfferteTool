using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.DAL.Models;

namespace OffertTemplateTool.DAL.Repositories
{
    public interface IRepository<T> where T: class, IDb
    {
        void Add(T item);
        IQueryable<T> GetAll();
        T Find(string key);
        bool Any(int key);
        T Find(int key);
        void Remove(int key);
        void Update(T item);
        void SaveChanges();

        Task AddAsync(T item);
        Task<IQueryable<T>> GetAllAsync();
        Task<T> FindAsync(string key);
        Task<bool> AnyAsync(string key);
        Task<T> FindAsync(int key);
        Task RemoveAsync(int key);
        Task UpdateAsync(T item);
        Task SaveChangesAsync();
    }
}
