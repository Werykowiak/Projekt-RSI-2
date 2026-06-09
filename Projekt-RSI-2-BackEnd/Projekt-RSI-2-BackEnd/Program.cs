using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Projekt_RSI_2_BackEnd.Data;
using Projekt_RSI_2_BackEnd.Hubs;
using Projekt_RSI_2_BackEnd.Interfaces;
using Projekt_RSI_2_BackEnd.Services;
using System.Text;
using System.Threading.RateLimiting;

namespace Projekt_RSI_2_BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)
                ));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowNuxt", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var jwtKey = builder.Configuration["Jwt:Key"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379"; 
                options.InstanceName = "TrainsCache_";    
            });

            builder.Services.AddSwaggerGen();

            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5, 
                            Window = TimeSpan.FromSeconds(10)
                        }));
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddSignalR();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });
            builder.Services.AddOpenApi();
            
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITrainRouteService, TrainRouteService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();
            app.UseRateLimiter();
            app.UseCors("AllowNuxt");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<BookingHub>("/bookingHub");

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var db = services.GetRequiredService<AppDbContext>();

                int retries = 0;
                const int maxRetries = 15;
                while (retries < maxRetries)
                {
                    try
                    {
                        logger.LogInformation("Próba zastosowania migracji (próba {Retries}/{MaxRetries})...", retries + 1, maxRetries);
                        db.Database.Migrate();
                        logger.LogInformation("Migracje zastosowane pomyślnie.");
                        break;
                    }
                    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1801)
                    {
                        logger.LogWarning("Baza danych już istnieje (błąd 1801). Próba kontynuacji...");
                        break;
                    }
                    catch (Exception ex)
                    {
                        retries++;
                        logger.LogError(ex, "Błąd podczas stosowania migracji. Ponawianie za 5 sekund...");
                        if (retries >= maxRetries) throw;
                        Thread.Sleep(5000);
                    }
                }
            }

            app.Run();
        }
    }
}
