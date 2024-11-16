using dotnet_sms_service.Models;
using dotnet_sms_service.Models.Configurations;
using dotnet_sms_service.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SMSApi.Api;
using System.Net;

namespace dotnet_sms_service.Functions
{
    public class CreateSmsNotifications
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISmsService _smsService;

        public CreateSmsNotifications(
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory,
            ISmsService smsService
        )
        {
            _logger = loggerFactory.CreateLogger<CreateSmsNotifications>();
            _httpClientFactory = httpClientFactory;
            _smsService = smsService;
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var httpClient = _httpClientFactory.CreateClient("sms-api-client");

            var dateForNotification = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
            var data = await httpClient.GetAsync($"visits/{dateForNotification}");

            if (data.StatusCode == HttpStatusCode.OK)
            {
                var deserializedVisits = JsonConvert.DeserializeObject<ApiResponse>(await data.Content.ReadAsStringAsync());

                foreach (Visit visit in deserializedVisits.Data)
                {
                    var response = _smsService.SendSms(visit);
                }
            }

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }        
    }
}
