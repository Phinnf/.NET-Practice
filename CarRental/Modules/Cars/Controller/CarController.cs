using CarRental.Modules.Cars.Interface;
using CarRental.Modules.Cars.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Modules.Cars.Controller
{
    [ApiController]
    [Route("api/cars")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        /// <summary>
        /// GET: api/cars
        /// GET: api/cars?startDate=2026-09-20&endDate=2026-09-25
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Car>>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var cars = await _carService.GetCarsAsync(startDate, endDate);
            return Ok(cars);
        }

        /// <summary>
        /// GET: api/cars/{id}
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Car>> GetById(Guid id)
            => Ok(await _carService.FindOneByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<Car>> Create([FromBody] Car car)
            => Ok(await _carService.CreateAsync(car));

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Car>> Update(Guid id, [FromBody] Car car)
            => Ok(await _carService.UpdateAsync(id, car));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _carService.DeleteAsync(id);
            return NoContent();
        }
    }
}
