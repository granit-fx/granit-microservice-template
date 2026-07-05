IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Infrastructure — persistent containers survive AppHost restarts
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();
var identityDb = postgres.AddDatabase("identity-db");
var catalogDb = postgres.AddDatabase("catalog-db");
var notificationDb = postgres.AddDatabase("notification-db");

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight();

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var keycloak = builder.AddKeycloak("keycloak", port: 8080)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithRealmImport("../../infra/keycloak-realms")
    // Disable HTTPS inside the container — dev uses HTTP on port 8080.
    // The health check uses the same endpoint; without this the self-signed cert
    // causes UntrustedRoot failures and Keycloak stays unhealthy.
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_HOSTNAME_STRICT_HTTPS", "false")
    .WithEnvironment("KC_PROXY", "edge");

// Local SMTP sink — captures emails sent by NotificationService and exposes a
// web UI on http://localhost:8025. Add Granit.Notifications.Email[.Smtp] +
// AddGranitNotificationsEmail()/EmailSmtp() in NotificationServiceModule to
// route notifications here.
var mailpit = builder.AddContainer("mailpit", "axllent/mailpit")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHttpEndpoint(port: 8025, targetPort: 8025, name: "ui")
    .WithEndpoint(port: 1025, targetPort: 1025, scheme: "tcp", name: "smtp");

// Migrations — run --migrate and exit before services start
var identityMigration = builder.AddProject<Projects.GranitMicroservice_IdentityService>("identity-migration")
    .WithReference(identityDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(identityDb)
    .WaitFor(rabbitmq)
    .WithArgs("--migrate");

var catalogMigration = builder.AddProject<Projects.GranitMicroservice_CatalogService>("catalog-migration")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(catalogDb)
    .WaitFor(rabbitmq)
    .WithArgs("--migrate");

var notificationMigration = builder.AddProject<Projects.GranitMicroservice_NotificationService>("notification-migration")
    .WithReference(notificationDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(notificationDb)
    .WaitFor(rabbitmq)
    .WithArgs("--migrate");

// Services — WaitFor ensures infrastructure + migrations are ready before services start
var identityService = builder.AddProject<Projects.GranitMicroservice_IdentityService>("identity-service")
    .WithReference(identityDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(identityDb)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak)
    .WaitForCompletion(identityMigration);

var catalogService = builder.AddProject<Projects.GranitMicroservice_CatalogService>("catalog-service")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(catalogDb)
    .WaitFor(rabbitmq)
    .WaitForCompletion(catalogMigration);

builder.AddProject<Projects.GranitMicroservice_NotificationService>("notification-service")
    .WithReference(notificationDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(notificationDb)
    .WaitFor(rabbitmq)
    .WaitFor(mailpit)
    .WaitForCompletion(notificationMigration);

builder.AddProject<Projects.GranitMicroservice_ApiGateway>("api-gateway")
    .WithReference(identityService)
    .WithReference(catalogService)
    .WithReference(redis)
    .WaitFor(identityService)
    .WaitFor(catalogService)
    .WaitFor(keycloak);

await builder.Build().RunAsync();
