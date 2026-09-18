using CarRental.Modules.Cars.Interface;
using CarRental.Modules.Cars.Models;
using CarRental.Modules.Cars.Repository;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Modules.Cars.Services
{
    public class CarService : CarRepository, ICarService
    {
        public CarService(DbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Car>> GetCarsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            // If no dates are provided, return all cars
            if (!startDate.HasValue && !endDate.HasValue)
            {
                return await FindAllAsync();
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

            return await FindAvailableCars(startDate.Value, endDate.Value);
        }

        public override async Task<Car?> FindOneByIdAsync(Guid id)
        {
            var car = await base.FindOneByIdAsync(id);
            return car ?? throw new KeyNotFoundException($"Car with ID {id} not found.");
        }

        public override async Task<Car?> UpdateAsync(Guid id, Car data)
        {
            var updated = await base.UpdateAsync(id, data);
            return updated ?? throw new KeyNotFoundException($"Car with ID {id} not found.");
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            var success = await base.DeleteAsync(id);
            if (!success)
            {
                throw new KeyNotFoundException($"Car with ID {id} not found.");
            }
            return true;
        }
    }
}
