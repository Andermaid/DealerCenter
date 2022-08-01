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
using System.Security.Claims;
using System.Threading.Tasks;

namespace DealerCenter.Controllers
{
    public class SalesManagerOrderController : Controller
    {
        DealerCenterContext db;

        public SalesManagerOrderController(DealerCenterContext context)
        {
            db = context;
        }

        public ActionResult SalesManagerOrders()
        {
            IQueryable<Order> order = db.Orders.Where(o => o.SalesManager.Login == User.Identity.Name ||
                                                o.PurchaseManager.Login == User.Identity.Name ||
                                                User.HasClaim(ClaimsIdentity.DefaultRoleClaimType, RoleEnum.Администратор.ToString()))
                                               .Where(o => o.OrderStatusId == (int)OrderStatusEnum.ПроверкаДоступностиЗакупки ||
                                                o.OrderStatusId == (int)OrderStatusEnum.ГотовКПередаче)
                                               .Include(o => o.Client)
                                               .Include(o => o.PurchaseManager)
                                               .Include(o => o.SalesManager)
                                               .Include(o => o.Machinery)
                                               .Include(o => o.Machinery.MachineryClass)
                                               .Include(o => o.Machinery.Supplier)
                                               .Include(o => o.OrderStatus)
                                               .Include(o => o.PaymentStatus);
            return View(order.ToList());
        }

        [Authorize(Roles = "Администратор, Продавец")]
        public IActionResult Create(int? machineryClassId, int? supplierId)
        {
            IQueryable<Machinery> machineries = db.Machineries.Where(m => !m.isDeleted);

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
            
            ViewBag.SalesManagers = new SelectList(db.Employees.Where(x => x.Role.Name == RoleEnum.Продавец.ToString() && !x.isDeleted).ToList(), "Id", "Name");
            ViewBag.PurchaseManagers = new SelectList(db.Employees.Where(x => x.Role.Name == RoleEnum.ОтветственныйЗаЗакупку.ToString() && !x.isDeleted).ToList(), "Id", "Name");
            ViewBag.Machineries = new SelectList(machineries, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateViewModel orderModel)
        {
            Order order = new Order();

            Client client = db.Clients.Where(c => c.Name == orderModel.ClientName && c.Passport == orderModel.ClientPassport).FirstOrDefault();
            if (client == null)
            {
                Client newClient = new Client { Name = orderModel.ClientName, Passport = orderModel.ClientPassport, PhoneNumber = orderModel.ClientPhoneNumber };
                db.Clients.Add(newClient);
                await db.SaveChangesAsync();
                client = db.Clients.Where(c => c.Name == orderModel.ClientName && c.Passport == orderModel.ClientPassport).FirstOrDefault();
            }
            order.ClientId = client.Id;
            order.SalesManagerId = orderModel.SalesManagerId;
            order.PurchaseManagerId = orderModel.PurchaseManagerId;
            order.MachineryId = orderModel.MachineryId;
            order.OrderStatus = db.OrderStatuses.Find((int)OrderStatusEnum.ПроверкаДоступностиЗакупки);
            order.PaymentStatus = db.PaymentStatuses.Find((int)PaymentStatusEnum.ОжиданиеПредоплаты);
            order.RegistrationDate = DateTime.Now;
            order.PurchasePrice = db.Machineries.Find(orderModel.MachineryId).PurchasePrice;
            order.SellingPrice = db.Machineries.Find(orderModel.MachineryId).SellingPrice;
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            return RedirectToAction("SalesManagerOrders");
        }

        [Authorize(Roles = "Администратор, Продавец")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (order != null)
                {
                    ViewBag.Clients = new SelectList(db.Clients.ToList(), "Id", "Name");
                    ViewBag.SalesManagers = new SelectList(db.Employees.Where(x => x.Role.Name == RoleEnum.Продавец.ToString()).ToList(), "Id", "Name");
                    ViewBag.PurchaseManagers = new SelectList(db.Employees.Where(x => x.Role.Name == RoleEnum.ОтветственныйЗаЗакупку.ToString()).ToList(), "Id", "Name");
                    ViewBag.Machineries = new SelectList(db.Machineries.ToList(), "Id", "Model");
                    return View(order);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Order order)
        {
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return RedirectToAction("SalesManagerOrders");
        }

        [Authorize(Roles = "Администратор, Продавец")]
        [HttpGet]
        [ActionName("ChangePaymentStatusToAwaitingFullPayment")]
        public async Task<IActionResult> ConfirmChangePaymentStatusToAwaitingFullPayment(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (order != null)
                {
                    return View(order);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePaymentStatusToAwaitingFullPayment(int? id)
        {
            Order order = db.Orders.Find(id);
            order.PaymentStatus = db.PaymentStatuses.Find((int)PaymentStatusEnum.ОжиданиеПолнойОплаты);
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return RedirectToAction("SalesManagerOrders");
        }

        [Authorize(Roles = "Администратор, Продавец")]
        [HttpGet]
        [ActionName("ChangePaymentStatusToFullyPaid")]
        public async Task<IActionResult> ConfirmChangePaymentStatusToFullyPaid(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (order != null)
                {
                    return View(order);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePaymentStatusToFullyPaid(int? id)
        {
            Order order = db.Orders.Find(id);
            order.PaymentStatus = db.PaymentStatuses.Find((int)PaymentStatusEnum.ПолностьюОплачен);
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return RedirectToAction("SalesManagerOrders");
        }

        [Authorize(Roles = "Администратор, Продавец")]
        [HttpGet]
        [ActionName("Close")]
        public async Task<IActionResult> ConfirmClose(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (order != null)
                {
                    return View(order);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Close(int? id)
        {
            Order order = db.Orders.Find(id);
            order.OrderStatus = db.OrderStatuses.Find((int)OrderStatusEnum.ЗаказЗакрыт);
            order.ClosingDate = DateTime.Now;
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return RedirectToAction("SalesManagerOrders");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.Include(o => o.Client)
                                             .Include(o => o.SalesManager)
                                             .Include(o => o.PurchaseManager)
                                             .Include(o => o.OrderStatus)
                                             .Include(o => o.PaymentStatus)
                                             .Include(o => o.Machinery)
                                             .Include(o => o.Machinery.MachineryClass)
                                             .Include(o => o.Machinery.Supplier)
                                             .FirstOrDefaultAsync(p => p.Id == id);
                if (order != null)
                    return View(order);
            }
            return NotFound();
        }

        [Authorize(Roles = "Администратор, Продавец")]
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (order != null)
                    return View(order);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (order != null)
                {
                    db.Orders.Remove(order);
                    await db.SaveChangesAsync();
                    return RedirectToAction("SalesManagerOrders");
                }
            }
            return NotFound();
        }
    }
}
