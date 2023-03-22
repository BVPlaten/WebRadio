using LibVLCSharp.Shared;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using WebRadio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3001") // Die URL der React-Anwendung
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Fügen Sie dies vor `var app = builder.Build();` hinzu
LibVLCSharp.Shared.Core.Initialize();

var libVlc = new LibVLC();
var mediaPlayer = new MediaPlayer(libVlc);

var startupStreamUrl = builder.Configuration["StartupStreamUrl"];
if (!string.IsNullOrEmpty(startupStreamUrl))
{
    try
    {
        var media = new Media(libVlc, startupStreamUrl, FromType.FromLocation);
        mediaPlayer.Play(media);
        // Warten Sie, bis der Stream gestartet wurde, bevor Sie die Anwendung starten.
        await Task.Delay(1000);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Fehler beim Starten des Streams: " + ex.Message);
    }
}

builder.Services.AddSingleton(_ => new MediaPlayerService(mediaPlayer, libVlc));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
