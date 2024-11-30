using dotnet_sms_service.Models.Configurations;
using dotnet_sms_service.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.Configure<ApiConnectionSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.Configure<SmsApiConfiguration>(builder.Configuration.GetSection("SmsApi"));

builder.Services.AddTransient<ISmsService, SmsService>();

builder.ConfigureFunctionsWebApplication();

builder.Services.AddHttpClient("groomer-backend-client", (serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiConnectionSettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.ApiUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("api-key", apiSettings.ApiKey);
});

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
