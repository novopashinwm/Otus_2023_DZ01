using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_01.Entities
{
    public class Project
    {
        public Project() 
        { 
        }
        public Project(long id, string? ProjectName, DateTime start, DateTime end, Employee employee) 
        {
            this.id = id;
            this.ProjectName = ProjectName;
            this.start = start;
            this.end = end;
            this.employee = employee;
        }

        private Employee? _employee;
        public long id { get; set; }
        public string? ProjectName { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public virtual Employee? employee { get => _employee; set => _employee = value; }

        public override string ToString()
        {
            return $"{id}, {ProjectName}, {start}-{end}, {employee}";
        }
    }
}
