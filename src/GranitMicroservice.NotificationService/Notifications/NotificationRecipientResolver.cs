using Granit.Notifications;
using Granit.Notifications.Abstractions;

namespace GranitMicroservice.NotificationService.Notifications;

/// <summary>
/// Resolves a recipient's contact information from a user identifier. Every Granit
/// notification channel (Email, SMS, …) depends on an <see cref="IRecipientResolver"/>;
/// Granit ships no default because recipient lookup is application-specific.
/// </summary>
/// <remarks>
/// <para>
/// This template runs NotificationService as a <b>separate</b> service from IdentityService,
/// so recipient lookup crosses a service boundary: a real implementation resolves contact
/// details from this service's own user read-model — synchronized from the identity service
/// via integration events — or from the originating notification payload. Replace the body
/// with that lookup and return <c>null</c> for unknown users so the channel can skip them.
/// This placeholder keeps the distributed pattern intact.
/// </para>
/// <para>
/// For a monolith / co-located deployment where <c>Granit.Identity</c> (and
/// <c>IIdentityUserReader</c>) runs in the SAME process as the notification pipeline, drop
/// this class and use the ready-made resolver from <c>Granit.Identity.Notifications</c>
/// instead: add the package reference and call <c>services.AddGranitIdentityRecipientResolver()</c>
/// (see granit-fx/granit-dotnet#2444). It does not fit here because this service has no
/// in-process identity store.
/// </para>
/// </remarks>
internal sealed class NotificationRecipientResolver : IRecipientResolver
{
    public Task<RecipientInfo?> ResolveAsync(string userId, CancellationToken cancellationToken = default)
    {
        RecipientInfo recipient = new()
        {
            UserId = userId,
            Email = $"{userId}@example.local",
            DisplayName = $"User {userId}",
            PreferredCulture = "en",
        };

        return Task.FromResult<RecipientInfo?>(recipient);
    }
}
