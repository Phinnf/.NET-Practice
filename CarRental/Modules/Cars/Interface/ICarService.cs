using CarRental.Modules.Cars.Models;

namespace CarRental.Modules.Cars.Interface
{
    public interface ICarService
    {
        Task<IReadOnlyList<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate);
    }
}
