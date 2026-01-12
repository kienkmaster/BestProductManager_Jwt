using BestProductManager.Api.Data;
using BestProductManager.Api.Entities;
using BestProductManager.Api.Repositories;
using BestProductManager.Services.Account;
using BestProductManager.Services.Products;
using BestProductManager.Services.Auth;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using BestProductManager.Services.Department;
using BestProductManager.Repositories;
using BestProductManager.Repository.Employee;
using BestProductManager.Services.Employee;
using BestProductManager.Api.Services.Admin;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext sử dụng SQL Server với connection string DefaultConnection.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' bắt buộc phải được thiết lập.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Cấu hình ASP.NET Core Identity với ApplicationUser và IdentityRole.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Cấu hình chính sách mật khẩu.
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cấu hình JwtOptions binding từ section "Jwt" trong appsettings.json.
builder.Services.Configure<BestProductManager.Models.JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

// Đọc thông tin cấu hình JWT từ appsettings.json.
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:Key"];

// Kiểm tra Jwt:Key bắt buộc phải được cấu hình.
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Cấu hình 'Jwt:Key' bắt buộc phải được thiết lập trong appsettings.json hoặc biến môi trường.");
}

var jwtCookieName = builder.Configuration["Jwt:CookieName"] ?? "BestProductManager.AuthToken";

// Tạo khóa ký JWT từ Jwt:Key.
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// Cấu hình Authentication sử dụng JWT Bearer, đọc token từ HttpOnly Cookie.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Lưu token vào AuthenticationProperties (phục vụ cho các kịch bản cần truy xuất token trên server).
        options.SaveToken = true;

        // Thiết lập quy tắc validate token.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };

        // Đọc JWT từ HttpOnly Cookie thay vì từ Authorization header.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue(jwtCookieName, out var token) &&
                    !string.IsNullOrWhiteSpace(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });


// Cấu hình Authorization mặc định dựa trên JWT.
builder.Services.AddAuthorization();

// Cấu hình CORS cho Angular dev server (http://localhost:4200) cho phép gửi cookie.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Đăng ký AutoMapper sử dụng assembly hiện tại.
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Đăng ký factory tạo SqlConnection dùng cho Dapper (stateless, dùng Singleton).
builder.Services.AddSingleton<ISqlConnectionFactory>(_ =>
    new SqlConnectionFactory(connectionString));

// Đăng ký các repository và service cho DI container.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAdminUsersService, AdminUsersService>();

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Đăng ký dịch vụ JWT dùng để tạo access token và refresh token.
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Đăng ký controller và cấu hình JSON.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger cho môi trường dev.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Cấu hình logging: giữ default provider + thêm EventLog (ghi vào Windows Event Viewer).
if (OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Bật CORS trước Authentication/Authorization.
app.UseCors("AllowAngularClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
