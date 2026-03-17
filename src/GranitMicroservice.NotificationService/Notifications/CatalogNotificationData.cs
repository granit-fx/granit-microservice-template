namespace GranitMicroservice.NotificationService.Notifications;

/// <summary>Payload sent when a new product is added to the catalog.</summary>
public sealed record ProductCreatedData(Guid ProductId, string ProductName, decimal Price);

/// <summary>Payload sent when an existing product is updated.</summary>
public sealed record ProductUpdatedData(Guid ProductId, string ProductName, decimal Price);
