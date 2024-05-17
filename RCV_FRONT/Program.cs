using Microsoft.AspNetCore.Authentication.Cookies;
using RCV_FRONT.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IServicio_API, Servicio_APIV>();

builder.Services.AddScoped<IServicioU_API, Servicio_APIU>();

builder.Services.AddHttpClient();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
       {
           options.Cookie.Name = "MiCookie";
           options.LoginPath = "/Inicio/Autenticar"; // Ruta para el inicio de sesión
           options.AccessDeniedPath = "/Inicio/AccessDenied"; // Ruta para el acceso denegado
           options.ExpireTimeSpan = TimeSpan.FromMinutes(15); // Tiempo de expiración de la cookie
       });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=IniciarSesion}/{id?}");

app.Run();
