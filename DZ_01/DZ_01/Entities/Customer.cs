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

        public Customer() { }

        public Customer(long id, string customerName, string address, Employee employee)
        {
            this.id = id;
            this.customerName = customerName;
            this.address = address;
            this.employee = employee;
        }

        public long id { get; set; }
        public string? customerName { get; set; }
        public string? address { get; set; }
        public virtual Employee? employee { get => _employee;  set =>  _employee = value;  }

        public override string ToString()
        {
            return $"{id}, {customerName}, {address}, {employee}";
        }
    }
}
