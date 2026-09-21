using CarRental.Core.Common;
using CarRental.Core.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CarRental.Core.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IReadOnlyList<T>> FindAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }
        public virtual async Task<T?> FindOneByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task<T?> FindOneAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
        }
        public virtual async Task<T> CreateAsync(T data)
        {
            if (data.Id == Guid.Empty)
            {
                data.Id = Guid.NewGuid();
            }
            data.CreatedAtUtc = DateTime.UtcNow;

            await _dbSet.AddAsync(data);
            await _context.SaveChangesAsync();
            return data;
        }
        public virtual async Task<T?> UpdateAsync(Guid id, T data)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null) return null;

            var originalCreatedAt = existing.CreatedAtUtc;

            _context.Entry(existing).CurrentValues.SetValues(data);
            existing.Id = id;
            existing.CreatedAtUtc = originalCreatedAt;
            existing.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }
        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null) return false;

            _dbSet.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
