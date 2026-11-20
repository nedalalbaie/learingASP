using learingASP.Configurations;
using learingASP.Data;
using learingASP.ExceptionHandling;
using learingASP.Models;
using learingASP.Repositories;
using learingASP.Services.Auth;
using learingASP.Services.Employees;
using learingASP.Services.LeaveRequests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// assigment shit
builder.Services.AddScoped<IEmployeesService, EmployeesService>();
builder.Services.AddScoped<ILeaveRequestsService, LeaveRequestsService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IPasswordHasher<Employee>,
    PasswordHasher<Employee>>();

//builder.Services.Configure<JwtSettings>(
//    builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(s => !string.IsNullOrWhiteSpace(s.Key)
        && Encoding.UTF8.GetByteCount(s.Key) >= 32,
        "JWT key must contain at least 32 bytes.")
    .Validate(s => !string.IsNullOrWhiteSpace(s.Issuer),
        "JWT issuer is required.")
    .Validate(s => !string.IsNullOrWhiteSpace(s.Audience),
        "JWT audience is required.")
    .Validate(s => s.AccessTokenExpirationMinutes > 0,
        "JWT expiration must be positive.")
    .Validate(s => s.RefreshTokenExpirationDays > 0,
        "Refresh token expiration must be positive.")
    .ValidateOnStart();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// the error handling stuff 
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


var app = builder.Build();


// some more error shit
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// artical says must be in this order
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
