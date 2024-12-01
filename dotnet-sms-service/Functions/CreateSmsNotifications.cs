using dotnet_sms_service.Models;
using dotnet_sms_service.Models.Notification;
using dotnet_sms_service.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;

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
        public async Task Run([TimerTrigger("0 16 11 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var httpClient = _httpClientFactory.CreateClient("groomer-backend-client");

            var dateForNotification = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
            var data = await httpClient.GetAsync($"visits/{dateForNotification}");

            if (data.StatusCode == HttpStatusCode.OK)
            {
                var deserializedVisits = JsonConvert.DeserializeObject<ApiResponse>(await data.Content.ReadAsStringAsync());

                List<NotificationResponse> responses = new List<NotificationResponse>();
                foreach (Visit visit in deserializedVisits.Data)
                {
                    var response = _smsService.SendSms(visit);
                    responses.Add(new NotificationResponse
                    {
                        CustomerId = visit.Customer.Id,
                        Message = response.Message,
                        SendDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"),
                        ApiResponse = response.List.First().Status,
                        ApiStatusCode = response.List.First().Status == "QUEUE" ? 200 : 500,
                    });
                }

                string json = JsonConvert.SerializeObject(responses);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync("save-notification-responses", content);

                    string responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response: {responseString}");
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }        
    }
}
