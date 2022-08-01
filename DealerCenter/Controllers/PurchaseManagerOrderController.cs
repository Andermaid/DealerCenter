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
    public class PurchaseManagerOrderController : Controller
    {
        DealerCenterContext db;

        public PurchaseManagerOrderController(DealerCenterContext context)
        {
            db = context;
        }

        public IActionResult PurchaseManagerOrders()
        {
            IQueryable<Order> order = db.Orders.Where(o => o.PurchaseManager.Login == User.Identity.Name ||
                                                o.SalesManager.Login == User.Identity.Name ||
                                                User.HasClaim(ClaimsIdentity.DefaultRoleClaimType, RoleEnum.Администратор.ToString()))
                                               .Where(o => o.PaymentStatusId == (int)PaymentStatusEnum.ОжиданиеПолнойОплаты)
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

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        [HttpGet]
        [ActionName("ChangeOrderStatusToWaitingForDelivery")]
        public async Task<IActionResult> ConfirmChangeOrderStatusToWaitingForDelivery(int? id)
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
        public async Task<IActionResult> ChangeOrderStatusToWaitingForDelivery(OrderSetDateViewModel orderModel)
        {
            Order order = db.Orders.Find(orderModel.Id);
            order.OrderStatus = db.OrderStatuses.Find((int)OrderStatusEnum.ОжиданиеДоставки);
            order.DeliveryDate = orderModel.DeliveryDate;
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return RedirectToAction("PurchaseManagerOrders");
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        [HttpGet]
        [ActionName("ChangeOrderStatusToReadyToTransfer")]
        public async Task<IActionResult> ConfirmChangeOrderStatusToReadyToTransfer(int? id)
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
        public async Task<IActionResult> ChangeOrderStatusToReadyToTransfer(OrderSetDateViewModel orderModel)
        {
            Order order = db.Orders.Find(orderModel.Id);
            order.OrderStatus = db.OrderStatuses.Find((int)OrderStatusEnum.ГотовКПередаче);
            order.DeliveryDate = DateTime.Now;
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return RedirectToAction("PurchaseManagerOrders");
        }

        [Authorize(Roles = "Администратор, ОтветственныйЗаЗакупку")]
        [HttpGet]
        [ActionName("Cancel")]
        public async Task<IActionResult> ConfirmCancel(int? id)
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
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (order != null)
                {
                    order.ClosingDate = DateTime.Now;
                    order.PaymentStatus = db.PaymentStatuses.Find((int)PaymentStatusEnum.ЗаказОтменён);
                    order.OrderStatus = db.OrderStatuses.Find((int)OrderStatusEnum.ЗаказОтменён);
                    db.Orders.Update(order);
                    await db.SaveChangesAsync();
                    return RedirectToAction("PurchaseManagerOrders");
                }
            }
            return NotFound();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
