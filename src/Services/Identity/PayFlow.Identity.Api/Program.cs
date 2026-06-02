using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PayFlow.Identity.Api.Application.Behaviors;
using PayFlow.Identity.Api.Application.Commands;
using PayFlow.Identity.Api.Domain.Entities;
using PayFlow.Identity.Api.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ---------------------------------------------------------------
// 1. CONTROLLERS & SWAGGER
// ---------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------------------------------------------
// 2. DATABASE
// ---------------------------------------------------------------
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb")));

// ---------------------------------------------------------------
// 3. ASP.NET CORE IDENTITY
// ---------------------------------------------------------------
builder.Services.AddIdentity<AppUser, IdentityRole>(options => { 
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<IdentityDbContext>()
  .AddDefaultTokenProviders();

// ---------------------------------------------------------------
// 4. JWT AUTHENTICATION
// ---------------------------------------------------------------
var jwtSecret = builder.Configuration["Jwt:SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
    };
});

// ---------------------------------------------------------------
// 5. MEDIATR
// ---------------------------------------------------------------
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

// ---------------------------------------------------------------
// 6. FLUENTVALIDATION
// ---------------------------------------------------------------
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommand>();

// ---------------------------------------------------------------
// 7. VALIDATION PIPELINE BEHAVIOR
// ---------------------------------------------------------------
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ---------------------------------------------------------------
// 8. APPLICATION SERVICES
// ---------------------------------------------------------------
builder.Services.AddScoped<ITokenService, TokenService>();


// ---------------------------------------------------------------
// BUILD
// ---------------------------------------------------------------
var app = builder.Build();

// Configure the HTTP request middleware pipeline.
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

