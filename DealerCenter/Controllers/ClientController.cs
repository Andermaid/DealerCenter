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
    public class ClientController : Controller
    {
        DealerCenterContext db;

        public ClientController(DealerCenterContext context)
        {
            db = context;
        }

        public IActionResult Clients()
        {
            IQueryable<Client> clients = db.Clients.Include(c => c.Orders);
            return View(clients.ToList());
        }

        [Authorize(Roles = "Администратор, Продавец")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            db.Clients.Add(client);
            await db.SaveChangesAsync();
            return RedirectToAction("Clients");
        }

        [Authorize(Roles = "Администратор, Продавец")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Client client = await db.Clients.FirstOrDefaultAsync(p => p.Id == id);
                if (client != null)
                {
                    return View(client);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Client client)
        {
            db.Clients.Update(client);
            await db.SaveChangesAsync();
            return RedirectToAction("Clients");
        }

        [Authorize(Roles = "Администратор, Продавец")]
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Client client = await db.Clients.FirstOrDefaultAsync(p => p.Id == id);
                if (client != null)
                    return View(client);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Client client = await db.Clients.FirstOrDefaultAsync(p => p.Id == id);
                if (client != null)
                {
                    db.Clients.Remove(client);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Clients");
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
                                                    .Where(o => o.ClientId == id)
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