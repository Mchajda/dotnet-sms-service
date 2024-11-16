using dotnet_sms_service.Models;
using dotnet_sms_service.Models.Configurations;
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
        private readonly SmsApiConfiguration _smsApiConfiguration;

        public CreateSmsNotifications(
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory,
            IOptions<SmsApiConfiguration> smsApiOptions
        )
        {
            _logger = loggerFactory.CreateLogger<CreateSmsNotifications>();
            _httpClientFactory = httpClientFactory;
            _smsApiConfiguration = smsApiOptions.Value;
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
                    SendSms(visit);
                }
            }

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }

        private void SendSms(Visit visit)
        {
            try
            {
                IClient client = new ClientOAuth(_smsApiConfiguration.SmsApiToken);

                var smsApi = new SMSFactory(client, new ProxyHTTP(_smsApiConfiguration.SmsApiUrl));

                var result =
                    smsApi.ActionSend()
                        .SetText("SMSAPI says hi!")
                        .SetTo(ParsePhoneNumber(visit.Customer.PhoneNumber))
                        .SetSender("Test")
                        .Execute();
            }
            catch (ActionException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ClientException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (HostException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ProxyException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private string ParsePhoneNumber(string phoneNumber)
        {
            throw new NotImplementedException();
        }
    }
}
