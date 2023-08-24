using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_01.Entities
{
    public class Project
    {
        private Employee? _employees;
        public long id { get; set; }
        public string? customerName { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public virtual Employee? employees { get => _employees; set => _employees = value; }
    }
}
