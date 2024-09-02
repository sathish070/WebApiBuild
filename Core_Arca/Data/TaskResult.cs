namespace Core_Arca.Data
{
    public class TaskResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string Content { get; set; }
        public int StatusCode { get; set; }
    }

    public class ErrorResponse
    {
        public string type { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public string traceId { get; set; }
    }
}
