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

        public async Task<IReadOnlyList<Car>> GetCarsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            // If no dates are provided, return all cars
            if (!startDate.HasValue && !endDate.HasValue)
            {
                return await _carRepository.FindAllAsync();
            }

            // Both dates must be provided together if date filtering is used
            if (!startDate.HasValue || !endDate.HasValue)
            {
                throw new ArgumentException("Both startDate and endDate must be provided together.");
            }

            if (startDate.Value >= endDate.Value)
            {
                throw new ArgumentException("StartDate must be earlier than EndDate.");
            }

            if (startDate.Value.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("StartDate cannot be in the past.");
            }

            return await _carRepository.FindAvailableCars(startDate.Value, endDate.Value);
        }

        public async Task<Car> FindOneByIdAsync(Guid id)
        {
            var car = await _carRepository.FindOneByIdAsync(id);
            return car ?? throw new KeyNotFoundException($"Car with ID {id} not found.");
        }

        public async Task<Car> CreateAsync(Car car)
        {
            return await _carRepository.CreateAsync(car);
        }

        public async Task<Car> UpdateAsync(Guid id, Car data)
        {
            var updated = await _carRepository.UpdateAsync(id, data);
            return updated ?? throw new KeyNotFoundException($"Car with ID {id} not found.");
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var success = await _carRepository.DeleteAsync(id);
            if (!success)
            {
                throw new KeyNotFoundException($"Car with ID {id} not found.");
            }
            return true;
        }
    }
}
