IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var postgres = builder.AddPostgres("postgres");
var identityDb = postgres.AddDatabase("identity-db");
var catalogDb = postgres.AddDatabase("catalog-db");
var notificationDb = postgres.AddDatabase("notification-db");

var redis = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.2")
    .WithArgs("start-dev", "--import-realm")
    .WithBindMount("../../infra/keycloak-realms", "/opt/keycloak/data/import")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
    .WithEnvironment("KC_HEALTH_ENABLED", "true")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin");

// Services
var identityService = builder.AddProject<Projects.GranitMicroservice_IdentityService>("identity-service")
    .WithReference(identityDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(identityDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

var catalogService = builder.AddProject<Projects.GranitMicroservice_CatalogService>("catalog-service")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(catalogDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq);

var notificationService = builder.AddProject<Projects.GranitMicroservice_NotificationService>("notification-service")
    .WithReference(notificationDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(notificationDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.GranitMicroservice_ApiGateway>("api-gateway")
    .WithReference(identityService)
    .WithReference(catalogService)
    .WithReference(redis)
    .WaitFor(identityService)
    .WaitFor(catalogService)
    .WaitFor(keycloak);

await builder.Build().RunAsync();
