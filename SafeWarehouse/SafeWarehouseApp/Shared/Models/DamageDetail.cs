namespace SafeWarehouseApp.Shared.Models
{
    public class DamageDetail
    {
        public string Id { get; set; } = default!;
        public File? Picture { get; set; }
        public string Description { get; set; } = default!;
    }
}