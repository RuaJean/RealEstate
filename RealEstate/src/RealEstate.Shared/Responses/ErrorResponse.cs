using System.Collections.Generic;

namespace RealEstate.Shared.Responses
{
    public class ErrorResponse
    {
        public string TraceId { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}


