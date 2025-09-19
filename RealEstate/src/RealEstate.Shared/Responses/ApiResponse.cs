using System.Diagnostics.CodeAnalysis;

namespace RealEstate.Shared.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory helpers para ergonomía en controladores")] 
        public static ApiResponse<T> Ok(T data, string message = "") => new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };

        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory helpers para ergonomía en controladores")] 
        public static ApiResponse<T> Fail(string message) => new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}


