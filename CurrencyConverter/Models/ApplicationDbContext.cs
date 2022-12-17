using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
           DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<Users> Users { get; set; }

        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }
    }
}
