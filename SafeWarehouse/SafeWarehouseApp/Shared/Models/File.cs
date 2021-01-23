namespace SafeWarehouseApp.Shared.Models
{
    public class File
    {
        public string Data { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;

        public string GetImageDataUrl() => $"data:{ContentType};base64,{Data}";
    }
}
