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
    public class SupplierController : Controller
    {
        DealerCenterContext db;

        public SupplierController(DealerCenterContext context)
        {
            db = context;
        }
        
        public IActionResult Suppliers()
        {
            IQueryable<Supplier> supplier = db.Suppliers.Include(s => s.Machineries);
            return View(supplier.ToList());
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();
            return RedirectToAction("Suppliers");
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Supplier supplier = await db.Suppliers.FirstOrDefaultAsync(p => p.Id == id);
                if (supplier != null)
                {
                    return View(supplier);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Supplier supplier)
        {
            db.Suppliers.Update(supplier);
            await db.SaveChangesAsync();
            return RedirectToAction("Suppliers");
        }


        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        [HttpGet]
        [ActionName("FlagDelete")]
        public async Task<IActionResult> ConfirmFlagDelete(int? id)
        {
            if (id != null)
            {
                Supplier supplier = await db.Suppliers.FirstOrDefaultAsync(p => p.Id == id);
                if (supplier != null)
                    return View(supplier);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> FlagDelete(int? id)
        {
            if (id != null)
            {
                Supplier supplier = await db.Suppliers.Include(s => s.Machineries).FirstOrDefaultAsync(p => p.Id == id);
                if (supplier != null)
                {
                    supplier.isDeleted = true;
                    db.Suppliers.Update(supplier);
                    await db.SaveChangesAsync();
                    foreach(var machinery in supplier.Machineries)
                    {
                        machinery.isDeleted = true;
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Suppliers");
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
                Supplier supplier = await db.Suppliers.FirstOrDefaultAsync(p => p.Id == id);
                if (supplier != null)
                    return View(supplier);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Supplier suppliers = await db.Suppliers.FirstOrDefaultAsync(p => p.Id == id);
                if (suppliers != null)
                {
                    db.Suppliers.Remove(suppliers);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Suppliers");
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
                Supplier supplier = await db.Suppliers.FirstOrDefaultAsync(p => p.Id == id);
                if (supplier != null)
                    return View(supplier);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id != null)
            {
                Supplier supplier = await db.Suppliers.FirstOrDefaultAsync(p => p.Id == id);
                if (supplier != null)
                {
                    supplier.isDeleted = false;
                    db.Suppliers.Update(supplier);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Suppliers");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                List<Machinery> machineries = await db.Machineries.Include(m => m.Supplier)
                                                                  .Include(m => m.MachineryClass)
                                                                  .Where(m => m.SupplierId == id)
                                                                  .ToListAsync();
                if (machineries != null)
                    return View(machineries);
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
