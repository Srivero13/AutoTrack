namespace AutoTrack.Api.Response;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Correcto(T data, string mensaje)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = mensaje,
            Data = data
        };
    }

    public static ApiResponse<T> Fallo(string mensaje, List<string> errores)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = mensaje,
            Errors = errores
        };
    }
}
