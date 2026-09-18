using CarRental.Core.Interface;
using CarRental.Modules.Cars.Models;
namespace CarRental.Modules.Cars.Interface
{
    public interface ICarRepository : IBaseRepository<Car>
    {
        Task<IReadOnlyList<Car>> FindAvailableCars(DateTime startDate, DateTime endDate);
    }
}