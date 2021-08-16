using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreTemplate.Domain.AggregatesModel;
using CoreTemplate.Domain.SeedWork;
using CoreTemplate.Infrastructure.NpgSql.Extensions;

namespace CoreTemplate.Infrastructure.NpgSql
{
    /// <summary>
    /// 
    /// </summary>
    public class InfrastructureContext : DbContext
    {
        private readonly IMediator _mediator;
        /// <summary>
        /// 测试用
        /// </summary>
        public virtual DbSet<AggregatesModelDemoEntity> AggregatesModelDemoEntities { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mediator"></param>
        public InfrastructureContext(DbContextOptions<InfrastructureContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
#if (pg)
            modelBuilder.Entity<AggregatesModelDemoEntity>()
                .HasQueryFilter(q => !q.IsDeleted)
                .HasIndex(p => new //这样可以自动创建索引
                {
                    p.City,
                    p.Name,
                    p.IsDeleted
                });
#endif
            base.OnModelCreating(modelBuilder);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries().Where(t => t.State == EntityState.Deleted || t.State == EntityState.Modified || t.State == EntityState.Added))
            {
                if (entry.Entity is Entity entityTrack)
                {
                    switch (entry.State)
                    {
                        case EntityState.Deleted:
                            {
                                entityTrack.DeletionTime = DateTimeOffset.Now;
                                entityTrack.IsDeleted = true;
                                entry.State = EntityState.Modified;
                                break;
                            }
                        case EntityState.Modified:
                            {
                                entityTrack.LastUpdateTime = DateTimeOffset.Now;
                                break;
                            }
                        case EntityState.Added:
                            {
                                entityTrack.CreationTime = DateTimeOffset.Now;
                                break;
                            }
                    }
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 

            bool saveResult = false;
            try
            {
                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = await base.SaveChangesAsync(cancellationToken);
                saveResult = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (saveResult)
                {
                    await _mediator.DispatchDomainEventsAsync(this);
                }
            }

            return saveResult;
        }
    }
}
