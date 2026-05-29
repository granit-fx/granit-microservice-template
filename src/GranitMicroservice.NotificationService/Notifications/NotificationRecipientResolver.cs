using Granit.Notifications;
using Granit.Notifications.Abstractions;

namespace GranitMicroservice.NotificationService.Notifications;

/// <summary>
/// Resolves a recipient's contact information from a user identifier. Every Granit
/// notification channel (Email, SMS, …) depends on an <see cref="IRecipientResolver"/>;
/// Granit ships no default because recipient lookup is application-specific.
/// </summary>
/// <remarks>
/// This template implementation is a placeholder. A real notification service resolves
/// contact details from its own user read-model — synchronized from the identity service
/// via integration events — or from the originating notification payload. Replace the body
/// with a lookup against your user directory and return <c>null</c> for unknown users so the
/// channel can skip them.
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
