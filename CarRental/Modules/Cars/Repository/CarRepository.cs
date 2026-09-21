using CarRental.Core.Repository;
using CarRental.Data;
using CarRental.Modules.Cars.Interface;
using CarRental.Modules.Cars.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Modules.Cars.Repository
{
    public class CarRepository : BaseRepository<Car>, ICarRepository
    {
        public CarRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IReadOnlyList<Car>> FindAvailableCars(DateTime startDate, DateTime endDate)
        {
            // Temporarily filter cars that are in stock (Stock > 0).
            // TODO: Add overlap check with bookings once the Booking module is implemented.
            // Example: exclude cars that have overlapping bookings in [startDate, endDate]:
            // !_context.Set<Booking>().Any(b => b.CarId == c.Id && b.StartDate < endDate && b.EndDate > startDate)
            var query = _dbSet.AsNoTracking()
                              .Where(c => c.Stock > 0);

            return await query.ToListAsync();
        }
    }
}
