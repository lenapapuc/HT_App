
    using Application.Commands.CreateMessage;
    using Infrastructure;
    using System.Reflection;
using Client;

    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    builder.Services.AddInfrastructure(configuration);
    // Add services to the container.

    builder.Services.AddControllers();



builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(typeof(Program).Assembly);
        config.RegisterServicesFromAssembly(typeof(CreateMessageHandler).Assembly);
    });
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("Client/_Host");

app.Run();
        
