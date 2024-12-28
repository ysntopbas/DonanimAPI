using DonanimAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using DonanimAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Environment variables'ları yükle - projenin root dizinindeki .env dosyasını oku
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

// CORS politikasını en başta tanımla
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("https://hardwareasyle.netlify.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// DI Setup
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService>();

// MongoDB Setup
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new ArgumentNullException("MONGODB_CONNECTION_STRING", "MongoDB connection string is not set in environment variables.");
    }
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");
    if (string.IsNullOrEmpty(databaseName))
    {
        throw new ArgumentNullException("MONGODB_DATABASE_NAME", "MongoDB database name is not set in environment variables.");
    }
    return client.GetDatabase(databaseName);
});

// JWT configuration
builder.Services.Configure<JwtSettings>(options =>
{
    options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
    options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
    options.Key = Environment.GetEnvironmentVariable("JWT_KEY");
});

var app = builder.Build();

// CORS'u ilk middleware olarak ekle (diğer middleware'lerden önce)
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Port ayarı
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();