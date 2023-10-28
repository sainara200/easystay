using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SlnWeb.DBContext;
using SlnWeb.Utilities;
using SlnWeb.Utilities.BrainTree;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("cn1")));

 
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();





 

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(need =>
{
    need.IdleTimeout = TimeSpan.FromMinutes(20);
    need.Cookie.HttpOnly = true;
    need.Cookie.IsEssential = true;
});
builder.Services.Configure<Configuracion>(builder.Configuration.GetSection("BrainTree"));
builder.Services.AddSingleton<IBrainTreeGate, BraimTreeGate>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
                     

app.UseHttpsRedirection();
app.UseStaticFiles();               

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=IndexHabitaciones}/{id?}");

app.Run();
