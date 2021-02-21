namespace SafeWarehouseApp.Shared.Models
{
    public class DamagePicture
    {
        public int Number { get; set; }
        public string? PictureId { get; set; }
        public string Description { get; set; } = default!;
    }
}