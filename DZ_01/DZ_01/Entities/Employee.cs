using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_01.Entities
{
    public class Employee
    {
        public long id { get; set; }
        public string? fullName { get; set; }
        public string? position { get; set; }
        public string? department { get; set; }
        public decimal salary { get; set; }
    }
}
