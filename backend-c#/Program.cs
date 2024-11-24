using backend_c_;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>( options =>
    options.UseNpgsql( builder.Configuration.GetConnectionString( "PostgreSqlConnection" ) ) );

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();
