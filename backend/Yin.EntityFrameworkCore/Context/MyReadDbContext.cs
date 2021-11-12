using Microsoft.EntityFrameworkCore;

namespace Yin.EntityFrameworkCore.Context
{
    public class MyReadDbContext : DbContext
    {
        public MyReadDbContext(DbContextOptions<MyReadDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyReadDbContext).Assembly);
        }
    }
}
