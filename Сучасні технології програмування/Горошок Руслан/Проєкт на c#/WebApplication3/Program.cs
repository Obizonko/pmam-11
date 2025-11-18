using Microsoft.EntityFrameworkCore;
using WebApplication3.EfCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EF_DataContext>(
                o => o.UseNpgsql(builder.Configuration.GetConnectionString("Ef_Postgres_Db"))
            );

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
