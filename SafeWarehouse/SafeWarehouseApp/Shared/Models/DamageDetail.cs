namespace SafeWarehouseApp.Shared.Models
{
    public class DamageDetail
    {
        public File Picture { get; set; } = new();
        public string Description { get; set; } = default!;
    }
}