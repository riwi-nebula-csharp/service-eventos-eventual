namespace service_eventos_eventual.Response;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}