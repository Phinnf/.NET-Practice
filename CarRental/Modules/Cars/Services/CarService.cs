using CarRental.Modules.Cars.Interface;
using CarRental.Modules.Cars.Models;

namespace CarRental.Modules.Cars.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<IReadOnlyList<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new ArgumentException("StartDate must be earlier than EndDate.");
            }

            if (startDate.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("StartDate cannot be in the past.");
            }

            return await _carRepository.FindAvailableCars(startDate, endDate);
        }
    }
}
