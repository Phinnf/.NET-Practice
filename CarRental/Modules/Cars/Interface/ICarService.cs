using CarRental.Modules.Cars.Models;

namespace CarRental.Modules.Cars.Interface
{
    public interface ICarService
    {
        Task<IReadOnlyList<Car>> GetCarsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Car> FindOneByIdAsync(Guid id);
        Task<Car> CreateAsync(Car car);
        Task<Car> UpdateAsync(Guid id, Car car);
        Task<bool> DeleteAsync(Guid id);
    }
}
