using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhetherData
{
    class EFContext : DbContext
    {
        private const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=WhetherData;Trusted_Connection=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        public DbSet<DatabaseLog> logs { get; set; }
    }
}
