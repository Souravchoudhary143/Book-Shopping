using E_comm_DataAccess.Data;
using E_comm_Utility;
using E_comm_DataAccess.Repository;
using E_comm_DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

//builder.Services.AddScoped < ICoverTypeRepository, CoverTypeRepository > ();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.Configure<StripeSettings> 
    (builder.Configuration.GetSection("StripeSettings"));

builder.Services.AddScoped<ISMSSender, SMSSender>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.AccessDeniedPath= $"/Identity/Account/AccessDenied";
    options.LogoutPath = $"/Identity/Account/Logout";
});


builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "435618969113795";
    options.AppSecret = "5dc42b0cb0986eea6647b3e479f17a85";
});


builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "417189030631-djjmrbisq2srccajcqsm88pffd7mftgj.apps.googleusercontent.com";
    options.ClientSecret = " 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 ";
});


builder.Services.AddAuthentication().AddTwitter(options =>
{

    options.ConsumerKey = "Jso8Q580qTzWERF7G6z20QaLV";
    options.ConsumerSecret = "y4jyjGrGZa8tK9LRsjnZnV0c98TqWVMUWRzFmfzbKy680YqWUX";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddAuthentication().AddTwilio(options =>
//{
//    options.AccountSid = " ";
//    options.AuthToken = "YourTwilioAuthToken";
//});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings")["Secretkey"];

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
