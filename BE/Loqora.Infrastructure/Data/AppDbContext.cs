using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Loqora.Application.Common.Interfaces;
using Loqora.Domain.Common;
using Loqora.Domain.Identity;
using Loqora.Infrastructure.Identity;

namespace Loqora.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : IdentityDbContext<AppUser, AppRole, Guid>(options), IAppDbContext
{


    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
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
