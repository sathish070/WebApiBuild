using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core_Arca.Models
{
    public class APIErrorLogs
    {
        [Key]
        public int ErrorId { get; set; }
        public string BuildType { get; set; }
        public string Method { get; set; }
        public string UniqueCode { get; set; }
        public string RequestedData { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Stacktrace { get; set; }
        public DateTime CreatedOn { get; set; }
        public string IpAddress { get; set; }
    }
}
