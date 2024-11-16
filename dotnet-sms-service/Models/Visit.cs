namespace dotnet_sms_service.Models
{
    public class ApiResponse
    {
        public required List<Visit> Data { get; set; }
    }
    public class Visit
    {
        public int Id { get; set; }
        public required Customer Customer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Customer
    {
        public required string Id { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
