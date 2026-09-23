namespace CarRental.Modules.Cars.Events
{
    public record CarCreatedEvent
    {
        public Guid Id { get; init; }
        public string Brand { get; init; } = string.Empty;
        public string CarModel { get; init; } = string.Empty;
        public decimal BasePrice { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
