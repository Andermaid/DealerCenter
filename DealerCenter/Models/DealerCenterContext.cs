using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DealerCenter.Models
{
    public class DealerCenterContext : DbContext
    {
        public DbSet<Order> Orders{ get; set; }
        public DbSet<Client> Clients{ get; set; }
        public DbSet<Employee> Employees{ get; set; }
        public DbSet<Machinery> Machineries{ get; set; }
        public DbSet<Supplier> Suppliers{ get; set; }
        public DbSet<MachineryClass> MachineryClasses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }

        public DealerCenterContext(DbContextOptions<DealerCenterContext> options)
          : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminUserLogin = "admin";
            string adminUserPassword = "admin";
            string adminUserName = "Администратор";
            string adminUserPhoneNumber = "911";

            PaymentStatus awaitingPrepayment = new PaymentStatus { Id = (int)PaymentStatusEnum.ОжиданиеПредоплаты, Name = PaymentStatusEnum.ОжиданиеПредоплаты.ToString()};
            PaymentStatus awaitingFullPayment = new PaymentStatus { Id = (int)PaymentStatusEnum.ОжиданиеПолнойОплаты, Name = PaymentStatusEnum.ОжиданиеПолнойОплаты.ToString() };
            PaymentStatus fullyPaid = new PaymentStatus { Id = (int)PaymentStatusEnum.ПолностьюОплачен, Name = PaymentStatusEnum.ПолностьюОплачен.ToString() };
            PaymentStatus paymentStatusOrderCanceled = new PaymentStatus { Id = (int)PaymentStatusEnum.ЗаказОтменён, Name = PaymentStatusEnum.ЗаказОтменён.ToString() };

            OrderStatus checkingForAwailability= new OrderStatus { Id = (int)OrderStatusEnum.ПроверкаДоступностиЗакупки, Name = OrderStatusEnum.ПроверкаДоступностиЗакупки.ToString() };
            OrderStatus waitingForDelivery= new OrderStatus { Id = (int)OrderStatusEnum.ОжиданиеДоставки, Name = OrderStatusEnum.ОжиданиеДоставки.ToString() };
            OrderStatus readyToTransfer= new OrderStatus { Id = (int)OrderStatusEnum.ГотовКПередаче, Name = OrderStatusEnum.ГотовКПередаче.ToString() };
            OrderStatus orderClosed= new OrderStatus { Id = (int)OrderStatusEnum.ЗаказЗакрыт, Name = OrderStatusEnum.ЗаказЗакрыт.ToString() };
            OrderStatus OrderStatusOrderCanceled= new OrderStatus { Id = (int)OrderStatusEnum.ЗаказОтменён, Name = OrderStatusEnum.ЗаказОтменён.ToString() };

            Role adminRole = new Role { Id = (int)RoleEnum.Администратор, Name = RoleEnum.Администратор.ToString() };
            Role salesManagerRole = new Role { Id = (int)RoleEnum.Продавец, Name = RoleEnum.Продавец.ToString() };
            Role purchaseManagerRole = new Role { Id = (int)RoleEnum.ОтветственныйЗаЗакупку, Name = RoleEnum.ОтветственныйЗаЗакупку.ToString() };

            Employee adminUser = new Employee { Id = 1, Login = adminUserLogin, Password = adminUserPassword, RoleId = adminRole.Id, Name = adminUserName, PhoneNumber = adminUserPhoneNumber };

            MachineryClass tracktor = new MachineryClass { Id = (int)MachineryClassEnum.Трактор, Name = MachineryClassEnum.Трактор.ToString() };
            MachineryClass bulldozer = new MachineryClass { Id = (int)MachineryClassEnum.Бульдозер, Name = MachineryClassEnum.Бульдозер.ToString() };
            MachineryClass excavator = new MachineryClass { Id = (int)MachineryClassEnum.Экскаватор, Name = MachineryClassEnum.Экскаватор.ToString() };

            modelBuilder.Entity<PaymentStatus>().HasData(new PaymentStatus[] { awaitingPrepayment, 
                                                                               awaitingFullPayment, 
                                                                               fullyPaid, 
                                                                               paymentStatusOrderCanceled });
            modelBuilder.Entity<OrderStatus>().HasData(new OrderStatus[] { checkingForAwailability, 
                                                                           waitingForDelivery, 
                                                                           readyToTransfer, 
                                                                           orderClosed, 
                                                                           OrderStatusOrderCanceled});
            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, salesManagerRole, purchaseManagerRole });
            modelBuilder.Entity<Employee>().HasData(new Employee[] { adminUser });
            modelBuilder.Entity<MachineryClass>().HasData(new MachineryClass[] { tracktor, bulldozer, excavator });

            base.OnModelCreating(modelBuilder);
        }

    }
}
