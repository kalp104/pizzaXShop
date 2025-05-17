using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Filters;

public class AuthorizePermissionOrderApp : ActionFilterAttribute
{
    private readonly IUserService _userService;

    public AuthorizePermissionOrderApp(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        string role = context.HttpContext.Items["UserRole"] as string ?? "";
        if(role == "Chef" || role == "AccountManager")
        {
            if(role == "Chef")
            {
                string path = context.HttpContext.Request.Path;
                if(path.Contains("Tables") || path.Contains("Menu") || path.Contains("WaitingList"))
                {
                    context.Result = new RedirectToActionResult("Privacy", "Home", null);
                    return;
                }
            }
            

        }else{
            context.Result = new RedirectToActionResult("Privacy", "Home", null);
            return;
        }
        
        await next();
    }
}
