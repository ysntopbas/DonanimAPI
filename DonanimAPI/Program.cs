using DonanimAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


// CORS Configurations
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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
var mongoConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:MongoDb");
var databaseName = builder.Configuration.GetValue<string>("ConnectionStrings:Database");

if (string.IsNullOrEmpty(mongoConnectionString))
{
    throw new ArgumentNullException("MongoDB connection string is missing.");
}

if (string.IsNullOrEmpty(databaseName))
{
    throw new ArgumentNullException("MongoDB database name is missing.");
}

// MongoClient ve IMongoDatabase'in DI'ye eklenmesi
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    return new MongoClient(mongoConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName);
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DonanimAPI v1");
        c.RoutePrefix = string.Empty;  // Swagger UI ana sayfası olarak ayarlıyoruz
    });
}

//// Enable CORS ONCEKI
//app.UseCors("AllowReactApp");
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// JWT middleware'ı eklemiyoruz çünkü AddJwtBearer bunu otomatik yapıyor.
app.UseHttpsRedirection();
app.UseAuthentication(); // JWT Authentication
app.UseAuthorization();
app.MapControllers();

app.Run();