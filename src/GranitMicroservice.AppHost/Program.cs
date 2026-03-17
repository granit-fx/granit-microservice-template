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
    .WithRealmImport("../../infra/keycloak-realms");

// Services — WaitFor ensures infrastructure is ready before services start
var identityService = builder.AddProject<Projects.GranitMicroservice_IdentityService>("identity-service")
    .WithReference(identityDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(identityDb)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

var catalogService = builder.AddProject<Projects.GranitMicroservice_CatalogService>("catalog-service")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(catalogDb)
    .WaitFor(rabbitmq);

var notificationService = builder.AddProject<Projects.GranitMicroservice_NotificationService>("notification-service")
    .WithReference(notificationDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(notificationDb)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.GranitMicroservice_ApiGateway>("api-gateway")
    .WithReference(identityService)
    .WithReference(catalogService)
    .WithReference(redis)
    .WaitFor(identityService)
    .WaitFor(catalogService)
    .WaitFor(keycloak);

await builder.Build().RunAsync();
