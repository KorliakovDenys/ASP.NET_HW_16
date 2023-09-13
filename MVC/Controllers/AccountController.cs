using System.Security.Claims;
using Library.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers;

[Authorize(Roles = "RegionalManager, CentralManager")]
public class AccountController : Controller {
    private readonly DataContext _dataContext;

    public AccountController(DataContext context) {
        _dataContext = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login() {
        return View();
    }
    
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model) {
        if (!ModelState.IsValid) return View(model);

        var user = await _dataContext.Users
            .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
        if (user != null) {
            await Authenticate(user);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Incorrect login or password");

        return View(model);
    }

    [HttpGet]
    public IActionResult Register() {
        return View();
    }

    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Register(RegisterModel model) {
    //     if (!ModelState.IsValid) return View(model);
    //     var user = await db.Users?.FirstOrDefaultAsync(u => u.Login == model.Login)!;
    //     if (user == null) {
    //         user = new User { Login = model.Login, Password = model.Password };
    //         var userRole = await db.Roles?.FirstOrDefaultAsync(r => r.Name == "user")!;
    //         if (userRole != null)
    //             user.Role = userRole;
    //
    //         db.Users?.Add(user);
    //         await db.SaveChangesAsync();
    //
    //         await Authenticate(user);
    //
    //         return RedirectToAction("Index", "Home");
    //     }
    //
    //     ModelState.AddModelError("", "Некорректные логин и(или) пароль");
    //
    //     return View(model);
    // }

    private async Task Authenticate(User user) {
        var role = user.Role.ToString();
        if (user.Role is Role.Manager) {
            var manager = await _dataContext.Managers!.Include(m => m.User).FirstAsync(m => m.UserId == user.Id);
            role = manager.Role + role;
        }
        var claims = new List<Claim> {
            new(ClaimsIdentity.DefaultNameClaimType, user.Login),
            new(ClaimsIdentity.DefaultRoleClaimType, role)
        };
        var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }
    
    [AllowAnonymous]
    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}