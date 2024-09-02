using System.ComponentModel.DataAnnotations;

namespace Core_Arca.Models
{
    public class TransmissionLogs
    {
        [Key]
        public int Id { get; set; }
        public required string BuildType { get; set; }
        public required string UrlMethod { get; set; }
        public required string UniqueCode { get; set; }
        public string? IpAddress { get; set; }
        public DateTime RequestedTime { get; set; }
        public DateTimeOffset RequestedTimeUtc { get; set; }
        public string? InboundData { get; set; }
        public string? InboundDataToSS { get; set; }
        public string? InboundUrl { get; set; }
        public string? InboundMethod { get; set; }
        public string? OutboundDataFromSS { get; set; }
        public int StatusCodeFromSS { get; set; }
        public string? OutboundData { get; set; }
        public int OutboundStatusCode { get; set; }
        public DateTime TransmissionDate { get; set; }
        public DateTimeOffset TransmissionDateUtc { get; set; }
    }
}
