using Microsoft.EntityFrameworkCore;
using WorkshopRSVP.Data;
using WorkshopRSVP.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// setting up EF Core with SQL Server connection
builder.Services.AddDbContext<EventManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// blob service for uploading banner images
builder.Services.AddSingleton<IBlobService, BlobService>();

var app = builder.Build();

// seed the database on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<EventManagerContext>();
        DbInitializer.Initialize(context);
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error seeding the database.");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
