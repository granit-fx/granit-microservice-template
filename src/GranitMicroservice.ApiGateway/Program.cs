using GranitMicroservice.ServiceDefaults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();
app.MapGet("/", () => new { Service = "ApiGateway" });

await app.RunAsync();
