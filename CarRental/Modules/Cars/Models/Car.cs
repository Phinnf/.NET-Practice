using CarRental.Core.Common;

namespace CarRental.Modules.Cars.Models
{
    public class Car : BaseEntity
    {
        public string Brand { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public int Seat { get; set; }
        public decimal BasePrice { get; set; }
        public int Stock { get; set; }
        public List<string> ImgUrls { get; set; } = new();

        // Used for Optimistic Concurrency (maps to PostgreSQL xmin)
        public uint Version { get; set; }
    }
}
