using Microsoft.EntityFrameworkCore;
using Demo3DAPI.Data;
using Demo3DAPI.Interfaces;
using Demo3DAPI.Services;
using Demo3DAPI.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPlayerAccountService, PlayerAccountService>();
builder.Services.AddScoped<IPlayerCharacterService, PlayerCharacterService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
