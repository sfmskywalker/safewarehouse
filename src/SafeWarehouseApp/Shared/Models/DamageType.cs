namespace SafeWarehouseApp.Shared.Models
{
    public sealed class DamageType : Document
    {
        public string? CustomerId { get; set; }
        public string Title { get; set; } = default!;
    }
}