
using LibraryManagement.Data;
using LibraryManagement.DTOs.Request;
using LibraryManagement.Middlewares;
using LibraryManagement.Profiles;
using LibraryManagement.Repositories;
using LibraryManagement.Services;
using LibraryManagement.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add DbContext with SQL Server
            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryManagement")));

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IBorrowedBookRepository, BorrowedBookRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IBorrowedBookService, BorrowedBookService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddHttpClient<GoogleAuthService>();

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(LibraryProfile));

            builder.Services.AddHostedService<OverdueCheckerService>();

            // Thêm cấu hình từ appsettings.json (có thể bỏ qua AddJsonFile, vì mặc định ASP.NET Core đã tự động làm điều này)
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Bind thông tin từ appsettings.json vào đối tượng JwtSettings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero, // Không cho phép sai lệch thời gian

                    RoleClaimType = ClaimTypes.Role
                };
            });

            // Thêm Authorization (phân quyền)
            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerGen(c =>
            {
                // ... (các cấu hình khác)

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            // Đăng ký JwtService cho IJwtService
            builder.Services.AddScoped<IJwtService, JwtService>();

            // Add custom services
            //builder.Services.AddApplicationServices();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
