using Granit.Auditing;
using Granit.Auditing.Domain;

namespace GranitMicroservice.ApiGateway.Internal;

/// <summary>
/// No-op <see cref="IAuditingCleaner"/> used when BFF endpoints pull in
/// <c>Granit.Auditing</c> transitively but the gateway has no audit
/// persistence wired. Keeps <c>AuditingCleanupWorker</c> happy without
/// dragging an EF Core companion + database into the gateway.
/// Register <c>Granit.Auditing.EntityFrameworkCore</c> and remove this when
/// audit retention is required in production.
/// </summary>
internal sealed class NullAuditingCleaner : IAuditingCleaner
{
    public Task<int> PurgeAsync(
        AuditCategory category,
        DateTimeOffset cutoff,
        int batchSize,
        CancellationToken cancellationToken = default) => Task.FromResult(0);

    public Task<int> PseudonymizeByUserAsync(
        string userId,
        CancellationToken cancellationToken = default) => Task.FromResult(0);
}
