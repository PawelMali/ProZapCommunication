using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using ProZapCommunication.Web.Components;
using ProZapCommunication.Web.Database;
using ProZapCommunication.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Konfiguracja uruchamiania jako Windows Service
// Ta metoda sprawia, że jeśli aplikacja zostanie zarejestrowana w systemie Windows jako usługa,
// będzie poprawnie zarządzać swoim cyklem życia (start/stop).
builder.Host.UseWindowsService();



// 2. Dodanie usług Blazor do kontenera DI (Dependency Injection)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ReportRepository>();
builder.Services.AddMudServices();
builder.Services.AddScoped<ExportService>();

// 3. Konfiguracja portów, na których aplikacja ma działać w sieci lokalnej
// Sprawdzamy, czy aplikacja NIE jest uruchomiona w trybie Development (Visual Studio).
// Jeśli działa jako produkcyjna usługa, wymuszamy konkretny port.
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://*:5000");
    // Gwiazdka (*) oznacza, że usługa będzie dostępna dla innych komputerów w sieci lokalnej po IP serwera
}
else
{
    // Dla Visual Studio możemy zostawić port domyślny lub też wymusić lokalny
    builder.WebHost.UseUrls("http://*:5000");
}

var app = builder.Build();

// 4. Konfiguracja potoku żądań HTTP (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // W środowisku produkcyjnym wymuszamy bezpieczne i stabilne zachowanie
    app.UseHsts();
}

app.UseStaticFiles();


app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
