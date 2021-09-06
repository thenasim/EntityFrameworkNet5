using EntityFrameworkNet5.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkNet5.Data
{
    public abstract class AuditableFootballLeagueDbContext : DbContext
    {
        public Task<int> SaveChangesAsync(string username)
        {
            var entries = ChangeTracker.Entries()
                .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var auditableObject = (BaseDomainObject)entry.Entity;
                auditableObject.UpdatedAt = DateTime.Now;
                auditableObject.UpdatedBy = username;

                if (entry.State == EntityState.Added)
                {
                    auditableObject.CreatedAt = DateTime.Now;
                    auditableObject.CreatedBy = username;
                }
            }

            return base.SaveChangesAsync();
        }
    }
}