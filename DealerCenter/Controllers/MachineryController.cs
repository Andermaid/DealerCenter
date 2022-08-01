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
    public class MachineryController : Controller
    {
        DealerCenterContext db;

        public MachineryController(DealerCenterContext context)
        {
            db = context;
        }

        public IActionResult Machineries(int? machineryClassId, int? supplierId)
        {
            IQueryable<Machinery> machineries = db.Machineries.Include(m => m.Supplier)
                                                              .Include(m => m.MachineryClass);

            List<MachineryClass> machineryClasses = db.MachineryClasses.ToList();
            machineryClasses.Insert(0, new MachineryClass { Id = 0, Name = "Все" });
            ViewBag.MachineryClasses = new SelectList(machineryClasses, "Id", "Name", machineryClassId);

            List<Supplier> suppliers = db.Suppliers.ToList();
            suppliers.Insert(0, new Supplier { Id = 0, Name = "Все" });
            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name", supplierId);

            if (machineryClassId != null && machineryClassId != 0)
            {
                machineries = machineries.Where(m => m.MachineryClassId == machineryClassId);
            }
            if (supplierId != null && supplierId != 0)
            {
                machineries = machineries.Where(m => m.SupplierId == supplierId);
            }
            return View(machineries.ToList());
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        public IActionResult Create()
        {
            ViewBag.Suppliers = new SelectList(db.Suppliers.ToList(), "Id", "Name");
            ViewBag.Classes = new SelectList(db.MachineryClasses.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Machinery machinery)
        {
            db.Machineries.Add(machinery);
            await db.SaveChangesAsync();
            return RedirectToAction("Machineries");
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Machinery machinery = await db.Machineries.Include(m => m.MachineryClass).FirstOrDefaultAsync(p => p.Id == id);
                if (machinery != null)
                {
                    ViewBag.Suppliers = new SelectList(db.Suppliers.ToList(), "Id", "Name");
                    ViewBag.Classes = new SelectList(db.MachineryClasses.ToList(), "Id", "Name");
                    return View(machinery);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Machinery machinery)
        {
            db.Machineries.Update(machinery);
            await db.SaveChangesAsync();
            return RedirectToAction("Machineries");
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        [HttpGet]
        [ActionName("FlagDelete")]
        public async Task<IActionResult> ConfirmFlagDelete(int? id)
        {
            if (id != null)
            {
                Machinery machinery = await db.Machineries.FirstOrDefaultAsync(p => p.Id == id);
                if (machinery != null)
                    return View(machinery);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> FlagDelete(int? id)
        {
            if (id != null)
            {
                Machinery machinery = await db.Machineries.FirstOrDefaultAsync(p => p.Id == id);
                if (machinery != null)
                {
                    machinery.isDeleted = true;
                    db.Machineries.Update(machinery);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Machineries");
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Machinery machinery = await db.Machineries.FirstOrDefaultAsync(p => p.Id == id);
                if (machinery != null)
                    return View(machinery);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Machinery machinery = await db.Machineries.FirstOrDefaultAsync(p => p.Id == id);
                if (machinery != null)
                {
                    db.Machineries.Remove(machinery);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Machineries");
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        [HttpGet]
        [ActionName("Restore")]
        public async Task<IActionResult> ConfirmRestore(int? id)
        {
            if (id != null)
            {
                Machinery machinery = await db.Machineries.FirstOrDefaultAsync(p => p.Id == id);
                if (machinery != null)
                    return View(machinery);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id != null)
            {
                Machinery machinery = await db.Machineries.FirstOrDefaultAsync(p => p.Id == id);
                if (machinery != null)
                {
                    machinery.isDeleted = false;
                    db.Machineries.Update(machinery);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Machineries");
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
                                                    .Where(o => o.MachineryId == id)
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
