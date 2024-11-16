using dotnet_sms_service.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace dotnet_sms_service.Functions
{
    public class CreateSmsNotifications
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateSmsNotifications(
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = loggerFactory.CreateLogger<CreateSmsNotifications>();
            _httpClientFactory = httpClientFactory;
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var httpClient = _httpClientFactory.CreateClient("sms-api-client");

            var dateForNotification = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
            var data = await httpClient.GetAsync($"visits/{dateForNotification}");

            var deserializedVisits = JsonConvert.DeserializeObject<ApiResponse>(await data.Content.ReadAsStringAsync());

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
