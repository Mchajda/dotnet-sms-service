using dotnet_sms_service.Models.Configurations;
using dotnet_sms_service.Models;
using SMSApi.Api;
using Microsoft.Extensions.Options;

namespace dotnet_sms_service.Services
{
    public interface ISmsService
    {
        public string ParsePhoneNumber(string phoneNumber);
        public SMSApi.Api.Response.Status SendSms(Visit visit);
    }
    public class SmsService
    {
        private readonly SmsApiConfiguration _smsApiConfiguration;
        public SmsService(
            IOptions<SmsApiConfiguration> smsApiOptions
        ) 
        {
            _smsApiConfiguration = smsApiOptions.Value;
        }
        public string ParsePhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Trim();

            if (!phoneNumber.StartsWith("48"))
                phoneNumber = $"48{phoneNumber}";

            return phoneNumber;
        }

        public SMSApi.Api.Response.Status SendSms(Visit visit)
        {
            SMSApi.Api.Response.Status response = null;

            try
            {
                IClient client = new ClientOAuth(_smsApiConfiguration.SmsApiToken);

                var smsApi = new SMSFactory(client, new ProxyHTTP(_smsApiConfiguration.SmsApiUrl));

                response =
                    smsApi.ActionSend()
                        .SetText("SMSAPI says hi!")
                        .SetTo(ParsePhoneNumber(visit.Customer.PhoneNumber))
                        .SetSender("Test")
                        .Execute();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }
    }
}
