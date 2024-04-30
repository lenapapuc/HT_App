using Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Net.Http; // Add this namespace

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddInfrastructure(configuration);
// Add services to the container.

builder.Services.AddMudServices();

// Add HttpClient service
builder.Services.AddScoped<HttpClient>(s =>
{
    var uriHelper = s.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(uriHelper.BaseUri) };
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();


app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
