using Dating_WebAPI.Interfaces;

namespace Dating_WebAPI.Helpers;
public class CloudinarySettings : ICloudinaryService
{
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;

    public CloudinarySettings CreateSettings(IConfiguration config)
    {
        return new CloudinarySettings() {
            CloudName = config["Cloudinary:CloudName"],
            ApiKey = config["Cloudinary:ApiKey"],
            ApiSecret = config["Cloudinary:ApiSecret"]            
        };
    }
}
