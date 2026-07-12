namespace Loqora.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedOnUtc { get; }
    string? DeletedBy { get; }

    void Delete(string? deletedBy = null, DateTimeOffset? deletedOnUtc = null);
    void Restore();
}
