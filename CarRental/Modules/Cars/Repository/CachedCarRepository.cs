using System.Linq.Expressions;
using CarRental.Modules.Cars.Interface;
using CarRental.Modules.Cars.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CarRental.Modules.Cars.Repository
{
    public class CachedCarRepository : ICarRepository
    {
        private readonly ICarRepository _innerRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedCarRepository> _logger;

        private const string AllCarsCacheKey = "cars:all";
        private static string GetCarByIdCacheKey(Guid id) => $"cars:{id}";

        private readonly MemoryCacheEntryOptions _defaultCacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

        public CachedCarRepository(
            ICarRepository innerRepository,
            IMemoryCache cache,
            ILogger<CachedCarRepository> logger)
        {
            _innerRepository = innerRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Car>> FindAllAsync()
        {
            if (_cache.TryGetValue(AllCarsCacheKey, out IReadOnlyList<Car>? cachedCars) && cachedCars != null)
            {
                _logger.LogInformation("Cache hit for all cars in repository [{Key}].", AllCarsCacheKey);
                return cachedCars;
            }

            _logger.LogInformation("Cache miss for all cars in repository [{Key}]. Fetching from database...", AllCarsCacheKey);
            var cars = await _innerRepository.FindAllAsync();
            _cache.Set(AllCarsCacheKey, cars, _defaultCacheOptions);
            return cars;
        }

        public async Task<Car?> FindOneByIdAsync(Guid id)
        {
            var cacheKey = GetCarByIdCacheKey(id);

            if (_cache.TryGetValue(cacheKey, out Car? cachedCar) && cachedCar != null)
            {
                _logger.LogInformation("Cache hit for car [{Key}] in repository.", cacheKey);
                return cachedCar;
            }

            _logger.LogInformation("Cache miss for car [{Key}] in repository. Fetching from database...", cacheKey);
            var car = await _innerRepository.FindOneByIdAsync(id);

            if (car != null)
            {
                _cache.Set(cacheKey, car, _defaultCacheOptions);
            }

            return car;
        }

        public Task<Car?> FindOneAsync(Expression<Func<Car, bool>> filter)
        {
            return _innerRepository.FindOneAsync(filter);
        }

        public async Task<Car> CreateAsync(Car data)
        {
            var created = await _innerRepository.CreateAsync(data);

            _logger.LogInformation("Invalidating [{Key}] cache after car creation.", AllCarsCacheKey);
            _cache.Remove(AllCarsCacheKey);

            return created;
        }

        public async Task<Car?> UpdateAsync(Guid id, Car data)
        {
            var updated = await _innerRepository.UpdateAsync(id, data);

            var cacheKey = GetCarByIdCacheKey(id);
            _logger.LogInformation("Invalidating [{DetailKey}] and [{AllKey}] caches after car update.", cacheKey, AllCarsCacheKey);
            _cache.Remove(cacheKey);
            _cache.Remove(AllCarsCacheKey);

            return updated;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var success = await _innerRepository.DeleteAsync(id);

            if (success)
            {
                var cacheKey = GetCarByIdCacheKey(id);
                _logger.LogInformation("Invalidating [{DetailKey}] and [{AllKey}] caches after car deletion.", cacheKey, AllCarsCacheKey);
                _cache.Remove(cacheKey);
                _cache.Remove(AllCarsCacheKey);
            }

            return success;
        }

        public Task<IReadOnlyList<Car>> FindAvailableCars(DateTime startDate, DateTime endDate)
        {
            return _innerRepository.FindAvailableCars(startDate, endDate);
        }
    }
}
