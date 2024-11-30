namespace dotnet_sms_service.Models.Notification
{
    public class NotificationResponse
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string Message { get; set; }
        public string SendDate { get; set; }
        public string ApiResponse { get; set; }
        public int ApiStatusCode { get; set; }
    }
}
