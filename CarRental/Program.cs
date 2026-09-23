using CarRental.Core.Interface;
using CarRental.Core.Middlewares;
using CarRental.Core.Repository;
using CarRental.Data;
using CarRental.Modules.Cars.Consumers;
using CarRental.Modules.Cars.Interface;
using CarRental.Modules.Cars.Repository;
using CarRental.Modules.Cars.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CarRental
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

            // IMemoryCache
            builder.Services.AddMemoryCache();

            // Repositories (Decorated with CachedCarRepository for In-Memory Caching)
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<CarRepository>();
            builder.Services.AddScoped<ICarRepository>(sp =>
                ActivatorUtilities.CreateInstance<CachedCarRepository>(sp, sp.GetRequiredService<CarRepository>()));

            // Services
            builder.Services.AddScoped<ICarService, CarService>();

            // MassTransit - RabbitMQ Message Queue
            builder.Services.AddMassTransit(x =>
            {
                // Đăng ký Consumer xử lý message
                x.AddConsumer<CarCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
                    var host = rabbitConfig.GetValue<string>("Host") ?? "localhost";
                    var port = rabbitConfig.GetValue<ushort?>("Port") ?? 5672;
                    var username = rabbitConfig.GetValue<string>("Username") ?? "guest";
                    var password = rabbitConfig.GetValue<string>("Password") ?? "guest";

                    cfg.Host(host, port, "/", h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    // Tự động tạo Queue và gán binding cho các Consumer
                    cfg.ConfigureEndpoints(context);
                });
            });

            // Controller
            builder.Services.AddControllers();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Global exception handling
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Car Rental API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            // Auto-create database & tables if they do not exist
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            app.Run();
        }
    }
}
