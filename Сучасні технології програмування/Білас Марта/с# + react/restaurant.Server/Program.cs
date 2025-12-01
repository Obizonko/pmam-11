using DataContext;
using DataContext.Seeds;
using Microsoft.EntityFrameworkCore;
using restaurant.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add cors policy
builder.Services.AddCustomCors(builder.Configuration, MyAllowSpecificOrigins);

// Add configurations 
builder.Services.AddCustomConfigurations(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

// Add swagger
builder.Services.AddCustomSwagger();

// Add db connection
builder.Services.AddCustomDbContext(builder.Configuration);

// Add identity
builder.Services.AddCustomIdentity();

// Add authentication bearer token
builder.Services.AddCustomAuthentication(builder.Configuration);

// Add services 
builder.Services.AddCustomServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<RestaurantDataContext>();
        await context.Database.MigrateAsync();
        await IdentitySeeder.EnsureDataSeeded(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders();
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None, // Set SameSite to None
    Secure = CookieSecurePolicy.Always, // Set Secure to Always
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
