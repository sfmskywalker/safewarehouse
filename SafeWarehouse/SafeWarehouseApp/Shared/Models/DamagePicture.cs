namespace SafeWarehouseApp.Shared.Models
{
    public class DamagePicture
    {
        public File? Picture { get; set; }
        public string Description { get; set; } = default!;
    }
}