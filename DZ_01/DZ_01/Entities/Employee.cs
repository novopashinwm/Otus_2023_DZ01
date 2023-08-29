using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_01.Entities
{
    public class Employee
    {
        public Employee()
        { 
        }
        public Employee(long id, string fullName, string position, string department, decimal salary)
        {
            this.id = id;
            this.fullName = fullName;
            this.position = position;
            this.department = department;
            this.salary = salary;
        }

        public long id { get; set; }
        public string? fullName { get; set; }
        public string? position { get; set; }
        public string? department { get; set; }
        public decimal salary { get; set; }

        public override string? ToString()
        {
            return $"{id}, {fullName}, {position}, {department}, {salary}";
        }
    }
}
