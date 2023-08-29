
using DZ_01.Entities;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB1
{
	public class DataContext: DbContext
	{
		public DbSet<Employee> employees { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<Customer> customers { get; set; }


        public DataContext()
        {
            Database.EnsureCreated();
        }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=localhost;Username=habrpguser;Password=pgpwd4habr;Database=habrdb");
			
			base.OnConfiguring(optionsBuilder);
		}
	}
}
