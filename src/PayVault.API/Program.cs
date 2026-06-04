using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Prometheus;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Text;
using PayVault.Infrastructure.Data;
using PayVault.Infrastructure.Repositories;
using PayVault.Infrastructure.Middleware;
using PayVault.Application.UseCases.Loans.Queries;
using PayVault.Infrastructure.Services;
using PayVault.Infrastructure.Identity;
using PayVault.Infrastructure.Auth;
using PayVault.Infrastructure;
using PayVault.Application.UseCases.Savings.Commands.CalculateInterest;
using PayVault.Application.Interfaces.Services;
using PayVault.Application.Interfaces.Repositories;
using PayVault.API.Middleware;
using PayVault.Application.UseCases.Loans;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using PayVault.Infrastructure.BackgroundServices;
using PayVault.Application.MappingProfiles;
using PayVault.Domain.Entities;
using AutoMapper;
using MediatR;
using DotNetEnv;
using HealthChecks.NpgSql;
using Serilog;


Env.Load();

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
})
.AddApplicationPart(typeof(Program).Assembly)
.AddControllersAsServices();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PayVault API",
        Version = "v1",
        Description = "BNPL + Savings Application API",
        Contact = new OpenApiContact
        {
            Name = "PayVault Team",
            Email = "support@payvault.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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


var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbName = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "PayVaultDb"; 
var dbUser = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "postgres";  
var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password";

var dbConnectionString = 
    $"Host={dbHost};" +
    $"Database={dbName};" +
    $"Username={dbUser};" +
    $"Password={dbPass};" +
    "SslMode=Require;" +                   
    "Trust Server Certificate=true;";

builder.Services.AddDbContext<AppDbContext>(options =>    
  options.UseNpgsql(dbConnectionString, npgsqlOptions =>
        npgsqlOptions.MigrationsAssembly("PayVault.Infrastructure")
                      .SetPostgresVersion(15, 0))); 


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();



builder.Services.Configure<SmtpSettings>(options =>
{
    options.Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
    options.Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
    options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "noreply@payvault.com";
    options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "your-app-password";
    options.From = Environment.GetEnvironmentVariable("SMTP_FROM") ?? "PayVault <noreply@payvault.com>";
});



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),

        NameClaimType = "nameid",
        RoleClaimType = "role"
    };

    options.MapInboundClaims = false;

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            Console.WriteLine("📥 TOKEN RECEIVED");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine("❌ AUTH FAILED: " + ctx.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
            Console.WriteLine("✅ TOKEN VALIDATED");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("AdminPolicy", policy => 
        policy.RequireClaim("role", "Admin")); 
});

builder.Services.AddFluentValidationAutoValidation();



builder.Services.AddAutoMapper(typeof(LoanProfile));


builder.Services.AddHostedService<InterestCalculationService>();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://seq:80")
    .CreateLogger();


builder.Host.UseSerilog();

builder.Services.AddInfrastructure();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<IEmailService, EmailService>(); 
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<ILoanEligibilityService, LoanEligibilityService>();
builder.Services.AddHttpClient<IPaymentService, PaystackPaymentService>();
builder.Services.AddScoped<ICreditScoreService, CreditScoreService>(); 
builder.Services.AddScoped<ISavingsPaymentService, SavingsPaymentService>();
builder.Services.AddScoped<ISavingsPaymentService, SavingsPaymentService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    AppDomain.CurrentDomain.GetAssemblies()
));
builder.Services.AddValidatorsFromAssemblies(
    AppDomain.CurrentDomain.GetAssemblies()
);

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "postgres",
        timeout: TimeSpan.FromSeconds(5)
    );

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

       
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        
        var adminEmail = "admin@payvault.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser 
            { 
                UserName = "admin@payvault.com",
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, "Hendrixnigga0799!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine(" Admin user created successfully!");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Seeding failed: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
     app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PayVault API V1");
        options.RoutePrefix = string.Empty;
        options.DisplayRequestDuration();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DisplayOperationId();
    });
}

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    Console.WriteLine($"🔍 Path: {context.Request.Path}");
    Console.WriteLine($"🔍 Auth: {authHeader?.Substring(0, 30)}...");
    Console.WriteLine($"🔍 User.Identity.Name: {context.User?.Identity?.Name}");
    Console.WriteLine($"🔍 User.IsAuthenticated: {context.User?.Identity?.IsAuthenticated}");
    await next();
});

app.MapGet("/", () => {
    Log.Information("Hello from PayVault API 🚀");
    return "Hello World";
});
// app.UseMiddleware<ExceptionMiddleware>();
// app.UseMiddleware<GlobalExceptionMiddleware>();
//app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.UseHttpMetrics();
app.MapMetrics();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();