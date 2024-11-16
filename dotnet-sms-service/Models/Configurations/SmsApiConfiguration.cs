namespace dotnet_sms_service.Models.Configurations
{
    public class SmsApiConfiguration
    {
        public required string SmsApiToken { get; set; }
        public required string SmsApiUrl { get; set; } = "https://api.smsapi.pl/";
    }
}
