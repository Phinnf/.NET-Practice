using CarRental.Modules.Cars.Models;

namespace CarRental.Modules.Cars.Interface
{
    public interface ICarService : ICarRepository
    {
        Task<IReadOnlyList<Car>> GetCarsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
