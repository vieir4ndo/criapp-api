#nullable enable

namespace CRIApp.Api.Controllers.Responses;

public class ApiResponse<T> where T : class?
{
    public bool Success { get; set; }
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public List<string> Messages { get; set; } = new();

    public ApiResponse(bool success, IEnumerable<T>? data = null, List<string>? messages = null)
    {
        Success = success;
        Data = data ?? Data;
        Messages = messages ?? Messages;
    }

    public ApiResponse(bool success, T data)
    {
        Success = success;
        Data = new List<T>() { data };
    }

    public ApiResponse(string? message = null)
    {
        Success = false;
        Messages = !string.IsNullOrEmpty(message) ? new List<string>() { message } : Messages;
    }
}