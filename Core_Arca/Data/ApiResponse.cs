using System.Net;

namespace Core_Arca.Data
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ApiResponse()
        {
            Success = true;
            StatusCode = HttpStatusCode.OK;
        }

        public ApiResponse(T data)
        {
            Success = true;
            Data = data;
            StatusCode = HttpStatusCode.OK;
        }

        public ApiResponse(HttpStatusCode statusCode, string errorMessage = null)
        {
            Success = false;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }
    }
}
