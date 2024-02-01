using Serilog;
using CatCryptor.WebApp.Services;
using CatCryptor.WebApp.Services.HostedServices;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddSingleton<FileCleanupService>();
builder.Services.AddHostedService<FileCleanupHostedService>();

builder.Services.AddScoped<FileProviderService>();
builder.Services.AddScoped<FileCryptorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CatCryptorLocalhostOrigin", builder =>
    {
        builder
            .WithOrigins("http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CatCryptorLocalhostOrigin");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();