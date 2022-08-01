using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DealerCenter.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ClaimsApp.Controllers
{
    public class AccountController : Controller
    {
        private DealerCenterContext db;
        public AccountController(DealerCenterContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if(User.IsInRole(RoleEnum.Администратор.ToString()))
            {
                return RedirectToAction("Employees", "Employee");
            }
            if (User.IsInRole(RoleEnum.Продавец.ToString()))
            {
                return RedirectToAction("SalesManagerOrders", "SalesManagerOrder");
            }
            if (User.IsInRole(RoleEnum.ОтветственныйЗаЗакупку.ToString()))
            {
                return RedirectToAction("PurchaseManagerOrders", "PurchaseManagerOrder");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Employee user = await db.Employees.Include(u => u.Role)
                    .FirstOrDefaultAsync(e => e.Login == model.Login && e.Password == model.Password);
                if (user != null && !user.isDeleted)
                {
                    await Authenticate(user);

                    if (user.Role.Name == RoleEnum.Администратор.ToString())
                    {
                        return RedirectToAction("Employees", "Employee");
                    }
                    if (user.Role.Name == RoleEnum.ОтветственныйЗаЗакупку.ToString())
                    {
                        return RedirectToAction("PurchaseManagerOrders", "PurchaseManagerOrder");
                    }
                    if (user.Role.Name == RoleEnum.Продавец.ToString())
                    {
                        return RedirectToAction("SalesManagerOrders", "SalesManagerOrder");
                    }
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(Employee user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name.ToString())
            };
            ClaimsIdentity claimIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
        }
    }
}