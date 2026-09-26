using Microsoft.AspNetCore.DataProtection;
using proyecto2.Configuracion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
string? rutaGraphviz = builder.Configuration["Graphviz:Ruta"];
if (rutaGraphviz == null)
{
    rutaGraphviz = "dot";
}
RegistroCatalogo.Registrar(builder.Services, rutaGraphviz, "Angel Daniel Ruiz Ramos", "C", "/Catalogo/Documentacion");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalogo}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
