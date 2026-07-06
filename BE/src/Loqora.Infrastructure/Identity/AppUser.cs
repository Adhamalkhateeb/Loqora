using Microsoft.AspNetCore.Identity;

namespace Loqora.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset LastModifiedUtc { get; private set; }

    private AppUser()
    {

    }

    private AppUser(string firstName, string lastName, string email)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = email;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        LastModifiedUtc = DateTimeOffset.UtcNow;
    }

    public static AppUser Create(
        string firstName,
        string lastName,
        string email)
    {
        return new AppUser(
            firstName.Trim(),
            lastName.Trim(),
            email.Trim().ToLowerInvariant());
    }

}
