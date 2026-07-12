using System.Linq.Expressions;

using Loqora.Application.Common.Interfaces;
using Loqora.Domain.Common;
using Loqora.Domain.Identity;
using Loqora.Infrastructure.Identity;

using MediatR;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Loqora.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : IdentityDbContext<AppUser, AppRole, Guid>(options), IAppDbContext
{

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.ApplySoftDeleteQueryFilters();
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Entity baseEntity && baseEntity.DomainEvents.Count != 0)
            .Select(e => (Entity)e.Entity)
            .ToList();

        var domainEvents = domainEntities.SelectMany(e => e.DomainEvents).ToList();

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }
    }
}


public static class Extensions
{
    public static ModelBuilder ApplySoftDeleteQueryFilters(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var body = Expression.Equal(property, falseConstant);
                var lambda = Expression.Lambda(body, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        return builder;
    }
}