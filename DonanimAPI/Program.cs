using MongoDB.Driver;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB Ayarlarını Yapılandır
var mongoDbSettings = builder.Configuration.GetSection("MongoDB");
var connectionString = mongoDbSettings["ConnectionString"];
var databaseName = mongoDbSettings["Database"];

// MongoDB Bağlantısını Hizmet Olarak Ekleyin
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    return new MongoClient(connectionString);
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins"); // CORS'u burada kullanın
app.UseAuthorization();
app.MapControllers();

app.Run();
