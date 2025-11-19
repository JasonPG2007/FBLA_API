using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBusiness
{
    public class FBLADbContext : DbContext
    {
        #region Constructor
        // Get DbContext from DI
        public FBLADbContext(DbContextOptions<FBLADbContext> options) : base(options)
        {
        }
        #endregion

        #region Entities
        public DbSet<Users> Users { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }
    }
}
