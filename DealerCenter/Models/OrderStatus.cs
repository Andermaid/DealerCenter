using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DealerCenter.Models
{
    public enum OrderStatusEnum : int
    {
        ПроверкаДоступностиЗакупки = 1,
        ОжиданиеДоставки,
        ГотовКПередаче,
        ЗаказЗакрыт,
        ЗаказОтменён
    }

    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        //==================================
        public List<Order> Orders { get; set; }

        public OrderStatus()
        {
            Orders = new List<Order>();
        }
    }
}
