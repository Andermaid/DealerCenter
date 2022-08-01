using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DealerCenter.Models
{
    public enum MachineryClassEnum : int
    {
        Трактор = 1,
        Бульдозер,
        Экскаватор
    }

    public class MachineryClass
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        //=====================================
        public List<Machinery> Machineries { get; set; }

        public MachineryClass()
        {
            Machineries = new List<Machinery>();
        }
    }
}
