using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_01.Entities
{
    public class Customer
    {
        private Employee? _employee;
        public long id { get; set; }
        public string? customerName { get; set; }
        public string? address { get; set; }
        public virtual Employee? Employee { get => _employee;  set =>  _employee = value;  }
    }
}
