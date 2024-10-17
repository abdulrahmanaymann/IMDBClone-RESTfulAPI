using System.Net;

namespace IMDbClone.Core.Responses
{
    public class APIResponse<T>
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }

        public APIResponse(HttpStatusCode statusCode, bool isSuccess, List<string>? errorMessages = null, T result = default!)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            ErrorMessages = errorMessages ?? new List<string>();
            Result = result;
        }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public List<string>? ErrorMessages { get; set; }

        public T Result { get; set; } = default!;

        // Factory method for creating success response
        public static APIResponse<T> CreateSuccessResponse(T result, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new APIResponse<T>(statusCode, true, result: result);
        }

        // Factory method for creating error response
        public static APIResponse<T> CreateErrorResponse(List<string> errorMessages, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new APIResponse<T>(statusCode, false, errorMessages: errorMessages);
        }
    }
}
