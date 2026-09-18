using System.ComponentModel.DataAnnotations;

namespace CarRental.Modules.Cars.DTOs
{
    public class SearchAvailableCarsRequest
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
