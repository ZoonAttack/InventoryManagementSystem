namespace Admin.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public int? StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null, int? code = null) =>
            new ApiResponse<T> { Success = true, Data = data, Message = message, StatusCode = code };

        public static ApiResponse<T> Fail(string? message = null, int? code = null) =>
            new ApiResponse<T> { Success = false, Message = message, StatusCode = code };
    }
}
