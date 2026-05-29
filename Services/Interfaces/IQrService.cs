namespace service_eventos_eventual.Services.Interfaces;

public interface IQrService
{
    /// <summary>
    /// Genera una imagen QR con el UUID dado y la sube a S3.
    /// Devuelve la URL pública del objeto en S3.
    /// </summary>
    Task<string> GenerateAndUploadAsync(string uuid);
}