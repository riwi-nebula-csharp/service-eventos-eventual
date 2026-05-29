using Amazon.S3;
using Amazon.S3.Transfer;
using QRCoder;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

public class QrService : IQrService
{
    private readonly IConfiguration _config;
    private readonly ILogger<QrService> _logger;

    public QrService(IConfiguration config, ILogger<QrService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<string> GenerateAndUploadAsync(string uuid)
    {
        // 1. Generar imagen QR en memoria (PNG)
        using var qrGenerator = new QRCodeGenerator();
        var qrData    = qrGenerator.CreateQrCode(uuid, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var pngBytes  = qrCode.GetGraphic(10); // 10px por módulo → ~350x350px

        // 2. Subir a S3
        var accessKey  = _config["Aws:AccessKeyId"]!;
        var secretKey  = _config["Aws:SecretAccessKey"]!;
        var region     = _config["Aws:Region"]!;
        var bucket     = _config["Aws:BucketName"]!;
        var folder     = _config["Aws:QrFolder"] ?? "qrcodes";

        var awsRegion  = Amazon.RegionEndpoint.GetBySystemName(region);
        var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);

        using var s3Client     = new AmazonS3Client(credentials, awsRegion);
        using var transferUtil = new TransferUtility(s3Client);
        using var stream       = new MemoryStream(pngBytes);

        var key = $"{folder}/{uuid}.png";

        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName  = bucket,
            Key         = key,
            InputStream = stream,
            ContentType = "image/png",
            // Objeto público para que el frontend pueda mostrarlo directamente
            CannedACL   = S3CannedACL.PublicRead
        };

        await transferUtil.UploadAsync(uploadRequest);

        var url = $"https://{bucket}.s3.{region}.amazonaws.com/{key}";
        _logger.LogInformation("QR subido a S3: {Url}", url);

        return url;
    }
}
