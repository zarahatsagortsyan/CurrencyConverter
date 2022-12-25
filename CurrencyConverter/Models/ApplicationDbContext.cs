using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
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
    public interface IApplicationDbContext: IDisposable
    {
        DbSet<Users> Users { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry Entry([NotNullAttribute] object entity);
        //Task<int> SaveChangesAsync();


    }


}
