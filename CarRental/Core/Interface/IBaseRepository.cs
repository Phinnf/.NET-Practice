using CarRental.Core.Common;
using System.Linq.Expressions;

namespace CarRental.Core.Interface
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> FindAllAsync();
        Task<T?> FindOneByIdAsync(Guid id);
        Task<T?> FindOneAsync(Expression<Func<T, bool>> filter);
        Task<T> CreateAsync(T data);
        Task<T?> UpdateAsync(Guid id, T data);
        Task<bool> DeleteAsync(Guid id);
    }
}
