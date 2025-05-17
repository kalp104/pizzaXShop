using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Core;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Implementations;
using PizzaShop.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserTableService, UserTableService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderAppService, OrderAppService>();

//excel
OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

// Filters
builder.Services.AddScoped<AuthorizePermissionUserTable>();
builder.Services.AddScoped<AuthorizePermissionRoles>();
builder.Services.AddScoped<AuthorizePermissionMenu>();
builder.Services.AddScoped<AuthorizePermissionSections>();
builder.Services.AddScoped<AuthorizePermissionTax>();
builder.Services.AddScoped<AuthorizePermissionOrders>();
builder.Services.AddScoped<AuthorizePermissionCustomer>();
builder.Services.AddScoped<AuthorizePermissionOrderApp>();
builder.Services.AddScoped<AuthorizePermissionUser>();


// Connection string + dependency injection
builder.Services.AddDbContext<PizzaShopContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(ICustomerRepository), typeof(CustomerRepository));
builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
builder.Services.AddScoped(typeof(ILoginRepository), typeof(LoginRepository));
builder.Services.AddScoped(typeof(IRoleRepository), typeof(RoleRepository));
builder.Services.AddScoped(typeof(ITaxRepository), typeof(TaxRepository));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IWaitingListRepository), typeof(WaitingListRepository));
builder.Services.AddScoped(typeof(ISectionRepository), typeof(SectionRepository));
builder.Services.AddScoped(typeof(ITableRepository), typeof(TableRepository));
builder.Services.AddScoped(typeof(IItemRepository), typeof(ItemRepository));
builder.Services.AddScoped(typeof(IItemRepository), typeof(ItemRepository));
builder.Services.AddScoped(typeof(IModifierRepository), typeof(ModifierRepository));
builder.Services.AddScoped(typeof(ICategoryRepository), typeof(CategoryRepository));
builder.Services.AddScoped(typeof(IModifierGroupRepository), typeof(ModifierGroupRepository));
builder.Services.AddScoped(typeof(IFeedBackRepository), typeof(FeedBackRepository));



// Add HttpContextAccessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Privacy"); // General error page for unhandled exceptions
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Custom TokenMiddleware for token validation from cookies
app.UseTokenMiddleware();

// Handle 404s by redirecting to /Home/Privacy
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 403)
    {
        context.HttpContext.Response.Redirect("/Home/Error403"); // Custom 403 page
    }
    else if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Home/Error404");
    }
    await Task.CompletedTask;
});

// Use built-in authentication and authorization (if configured)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
