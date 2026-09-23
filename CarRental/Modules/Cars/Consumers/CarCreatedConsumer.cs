using MassTransit;
using CarRental.Modules.Cars.Events;

namespace CarRental.Modules.Cars.Consumers
{
    public class CarCreatedConsumer : IConsumer<CarCreatedEvent>
    {
        private readonly ILogger<CarCreatedConsumer> _logger;

        public CarCreatedConsumer(ILogger<CarCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CarCreatedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "\n======================================================\n" +
                " [x] Message Queue: Received CarCreatedEvent!\n" +
                "     - Car ID: {CarId}\n" +
                "     - Brand: {Brand}\n" +
                "     - Model: {Model}\n" +
                "     - Base Price: {Price:C}\n" +
                "     - Created At: {CreatedAt}\n" +
                "======================================================",
                message.Id, message.Brand, message.CarModel, message.BasePrice, message.CreatedAt);

            // Giả lập tác vụ xử lý nền không đồng bộ (gửi email thông báo, đồng bộ dữ liệu thống kê, etc.)
            await Task.Delay(100);

            _logger.LogInformation(" [v] Finished processing background tasks for Car ID: {CarId}", message.Id);
        }
    }
}

