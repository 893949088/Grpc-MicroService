using Grpc.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.MicroService.EF
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var typesToRegister = AppDomain.CurrentDomain.GetAssemblies()
                .Select(p => p.GetTypes().Where(t => typeof(BaseEntity).IsAssignableFrom(t) && !t.IsAbstract)).ToArray();

            //var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            //     .Where(p => p.BaseType == typeof(BaseEntity));

            foreach (var types in typesToRegister)
                foreach (var type in types)
                {
                    modelBuilder.Entity(type);
                }
        }
    }
}
