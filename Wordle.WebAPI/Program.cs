using AspNetCoreRateLimit;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;
using Wordle.Application.Common.Behaviors;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.DailyWords.Commands.Add;
using Wordle.Application.Users.Commands.Register;
using Wordle.Application.Users.Commands.ResetPassword;
using Wordle.Domain.Common;
using Wordle.Infrastructure.Auth;
using Wordle.Infrastructure.Common;
using Wordle.Infrastructure.CurrentUser;
using Wordle.Infrastructure.Data;
using Wordle.Infrastructure.JsonConverters;
using Wordle.Infrastructure.Mail;
using Wordle.Infrastructure.Repositories;
using Wordle.Infrastructure.Security;
using Wordle.WebAPI.Middlewares;
using Npgsql.EntityFrameworkCore.PostgreSQL;


var builder = WebApplication.CreateBuilder(args);

// Serilog konfigürasyonu
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true
        })
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// AppSettings okuma
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// SMTP
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEMailService, SmtpEmailService>();

// Controller
builder.Services.AddControllers();

// CORS – Her yerden eriþim saðlamak için (yayýnda gerekebilir, sýkýlaþtýrýlabilir)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Wordle API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT için 'Bearer {token}' þeklinde girin"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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


// CQRS, Validations, AutoMapper
builder.Services.AddMediatR(typeof(RegisterUserCommand).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ResetPasswordValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddDailyWordCommandValidator>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// DB
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<WordleDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    builder.Services.AddDbContext<WordleDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}


// Repositories ve Servisler
builder.Services.AddScoped<IDailyWordRepository, EfDailyWordRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IGuessRepository, EFGuessRepository>();
builder.Services.AddScoped<IScoreRepository, EfScoreRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

//JWT
var jwtKey = builder.Configuration["PROD_JWT_KEY"]
             ?? builder.Configuration["Jwt:Key"]; 

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!))
        };
    });


// JSON Converter
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

// Current User
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// App baþlat
Log.Information("Uygulama baþlatýlýyor...");
var app = builder.Build();

// Middleware ve sýralamalar
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger - hem dev hem prod'da açýk
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wordle API v1");
    c.RoutePrefix = "swagger"; // URL: /swagger
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Bunu býrakabilirsin ya da kaldýrabilirsin, optional.
}

// CORS middleware – Authentication’tan önce gelmeli
app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WordleDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();
app.MapControllers();

app.Run();

