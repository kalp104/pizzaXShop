using System.Diagnostics;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Models;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILoginService _loginService;

    public HomeController(
        ILoginService loginService,
        IEmailService emailService,
        IConfiguration configuration
    )
    {
        _emailService = emailService;
        _configuration = configuration;
        _loginService = loginService;
    }

    #region Login
    public IActionResult Index()
    {
        if (
            HttpContext.Request.Cookies["auth_token"] != null
            && HttpContext.Items["UserRole"] != null
        )
        {
            var role = HttpContext.Items["UserRole"] as string;
            if (role == "Admin" || role == "AccountManager")
            {
                TempData["success"] = "logged in";
                return RedirectToAction("UserDashboard", "Users");
            }else if(role == "Chef"){
                TempData["success"] = "logged in";
                return RedirectToAction("Index", "OrderApp");
            }
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            ResponseTokenViewModel? response = await _loginService.GetLoginService(model);
            if (response != null && response.token.Length > 0)
            {
                SetJwtCookie(response.token, model.Rememberme);
                return RedirectToAction("RoleWiseBack", "Users");
            }
            TempData["EmailWrong"] = response.response;
        }
        return View(model);
    }

    private void SetJwtCookie(string token, bool isPersistent)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = isPersistent ? DateTime.Now.AddDays(30) : DateTime.Now.AddDays(1), // Session cookie if not persistent
        };
        Response.Cookies.Append("auth_token", token, cookieOptions);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token");
        TempData["logout"] = "Logout Successful!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult UpdateEmail(string email)
    {
        TempData["Email"] = email;

        return Ok();
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {

        ViewBag.Email = TempData["Email"];
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(EmailViewModel Email)
    {
            if (Email == null || string.IsNullOrEmpty(Email.ToEmail))
            {
                TempData["ErrorMessage"] = "Email address is required.";
            }
            Account? account = await _loginService.GetAccoutAsync(Email.ToEmail);
            if (account == null)
            {
                TempData["ErrorMessage"] = "No account found with this email.";
                return View(Email);
            }
            try{
                // Create new password reset request
                bool res = await _loginService.ForgetPasword(Email.ToEmail);

                if(res == false)
                {
                    TempData["ErrorMessage"] = "No account found with this email.";
                    return View(Email);
                }

                TempData["validEmail"] = Email.ToEmail;
                TempData["SuccessMessage"] = "Password reset instructions have been sent to your email.";
                TempData.Keep("validEmail");
                return View(Email);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to send email. Please try again later.";
                return View(Email);
            }
    }

    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Invalid password reset link.";
            return RedirectToAction("ForgetPassword");
        }

            PasswordResetRequest? resetRequest = await _loginService.ResetPasswordGetService(token);
    
                if (resetRequest == null)
                {
                    TempData["ErrorMessage"] = "Invalid or expired password reset link.";
                    return RedirectToAction("ForgetPassword");
                }

                // Check if request is still open (CloseDate is null)
                if (resetRequest.Closedate != null)
                {
                    TempData["ErrorMessage"] = "This password reset link has been expire.";
                    return RedirectToAction("ForgetPassword");
                }

                // Check if request is within 24 hours
                if (resetRequest.Createdate < DateTime.Now.AddHours(-24))
                {
                    TempData["ErrorMessage"] = "This password reset link has expired.";
                    return RedirectToAction("ForgetPassword");
                }

                // Get the email from UserId
                Account? account = await _loginService.GetAccountById(resetRequest.Userid);
                if (account == null)
                {
                    TempData["ErrorMessage"] = "Account not found.";
                    return RedirectToAction("ForgetPassword");
                }

        return View(new ForgetPasswordViewModel { Token = token, Email = account.Email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ForgetPasswordViewModel model)
    {
        try{
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            PasswordResetRequest? resetRequest = await _loginService.ResetPasswordGetService(model.Token);

            if (resetRequest == null)
            {
                TempData["ErrorMessage"] = "Invalid or expired password reset link.";
                return RedirectToAction("ForgetPassword");
            }

            // Check if request is still open
            if (resetRequest.Closedate != null)
            {
                TempData["ErrorMessage"] = "This password reset link has already been used.";
                return RedirectToAction("ForgetPassword");
            }

            // Check if request is within 24 hours
            if (resetRequest.Createdate < DateTime.Now.AddHours(-24))
            {
                TempData["ErrorMessage"] = "This password reset link has expired.";
                return RedirectToAction("ForgetPassword");
            }


            if (ModelState.IsValid)
            {
                string? response = await _loginService.ResetPasswordService(model);

                switch (response)
                {
                    case "1":
                        TempData["password"] = "account does not exist";
                        break;
                    case "2":
                        TempData["EmailNotMatch"] = "email doesnot match";
                        break;
                    case "3":
                        TempData["password"] = "password can not be same as previous one";
                        break;
                    case "4":
                        TempData["success"] = "successfully changed password";
                        return RedirectToAction("Index");
                    default:
                        TempData["password"] = "invalid";
                        break;
                }
            }
            TempData["error"] = "please confirm password first";
            return View(model);
        }catch (Exception ex)
        {
            Console.WriteLine($"Failed to reset password: {ex.Message}");
            TempData["ErrorMessage"] = "Failed to reset password. Please try again later.";
            return View(model);
        }
    }

    #endregion
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error404()
    {
        return View();
    }

   
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
