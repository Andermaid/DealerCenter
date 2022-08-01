using DealerCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DealerCenter.Controllers
{
    public class EmployeeController : Controller
    {
        DealerCenterContext db;

        public EmployeeController(DealerCenterContext context)
        {
            db = context;
        }

        [Authorize(Roles = "Администратор")]
        public IActionResult Employees()
        {
            IQueryable<Employee> employees = db.Employees.Include(e => e.Role);
            return View(employees.ToList());
        }

        [Authorize(Roles = "Администратор")]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(db.Roles.Where(r => r.Name != RoleEnum.Администратор.ToString()), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            db.Employees.Add(employee);
            await db.SaveChangesAsync();
            return RedirectToAction("Employees");
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Employee employee = await db.Employees.Include(e => e.Role).FirstOrDefaultAsync(p => p.Id == id);
                if (employee != null)
                {
                    if (employee.Role.Name == RoleEnum.Администратор.ToString())
                    {
                        ViewBag.Roles = new SelectList(db.Roles.Where(r => r.Name == RoleEnum.Администратор.ToString()), "Id", "Name");
                    }
                    else
                    {
                        ViewBag.Roles = new SelectList(db.Roles.Where(r => r.Name != RoleEnum.Администратор.ToString()), "Id", "Name");
                    }
                    return View(employee);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            db.Employees.Update(employee);
            await db.SaveChangesAsync();
            return RedirectToAction("Employees");
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("FlagDelete")]
        public async Task<IActionResult> ConfirmFlagDelete(int? id)
        {
            if (id != null)
            {
                Employee employee = await db.Employees.FirstOrDefaultAsync(p => p.Id == id);
                if (employee != null)
                    return View(employee);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> FlagDelete(int? id)
        {
            if (id != null)
            {
                Employee employee = await db.Employees.FirstOrDefaultAsync(p => p.Id == id);
                if (employee != null)
                {
                    employee.isDeleted = true;
                    db.Employees.Update(employee);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Employees");
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("Restore")]
        public async Task<IActionResult> ConfirmRestore(int? id)
        {
            if (id != null)
            {
                Employee employee = await db.Employees.FirstOrDefaultAsync(p => p.Id == id);
                if (employee != null)
                    return View(employee);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id != null)
            {
                Employee employee = await db.Employees.FirstOrDefaultAsync(p => p.Id == id);
                if (employee != null)
                {
                    employee.isDeleted = false;
                    db.Employees.Update(employee);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Employees");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                List<Order> orders = await db.Orders.Include(o => o.Client)
                                                    .Include(o => o.SalesManager)
                                                    .Include(o => o.PurchaseManager)
                                                    .Include(o => o.OrderStatus)
                                                    .Include(o => o.PaymentStatus)
                                                    .Include(o => o.Machinery)
                                                    .Include(o => o.Machinery.MachineryClass)
                                                    .Include(o => o.Machinery.Supplier)
                                                    .Where(o => o.SalesManagerId == id || o.PurchaseManagerId == id)
                                                    .ToListAsync();
                if (orders != null)
                    return View(orders);
            }
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
