namespace SafeWarehouseApp.Shared.Models
{
    public sealed class Material : Document
    {
        public string? CustomerId { get; set; }
        public string Name { get; set; } = default!;
    }
}