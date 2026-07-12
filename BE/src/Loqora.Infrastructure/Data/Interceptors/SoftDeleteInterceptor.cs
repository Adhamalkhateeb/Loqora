using Loqora.Application.Common.Interfaces;
using Loqora.Domain.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Loqora.Infrastructure.Data.Interceptors;

public sealed class SoftDeleteInterceptor(
    IUser user,
    TimeProvider timeProvider)
    : SaveChangesInterceptor
{
    private readonly IUser _user = user;
    private readonly TimeProvider _timeProvider = timeProvider;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SoftDeleteEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SoftDeleteEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SoftDeleteEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = _timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.Entity.Delete(_user.Id, utcNow);
            entry.State = EntityState.Modified;
        }
    }
}