namespace Core_Arca.Models
{
    public class ArcaRequest
    {
        public int omahaRequestId { get; set; }
        public string arcaTicketId { get; set; }
        public string testOrLive { get; set; }
        public string serial { get; set; }
        public string model { get; set; }
        public string problemSummary { get; set; }
        public string contactFirstName { get; set; }
        public string contactLastName { get; set; }
        public string contactPhone { get; set; }
        public string contactExtension { get; set; }
        public string contactEmail { get; set; }
        public string serviceSiteName { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string note { get; set; }
        public string processingIssues { get; set; }
        public string omahaTicketId { get; set; }
        public string transmissionResult { get; set; }
        public string transmissionTimestamp { get; set; }
    }
}
