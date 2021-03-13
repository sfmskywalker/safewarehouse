namespace SafeWarehouseApp.Shared.Models
{
    public abstract class Document
    {
        public string Id { get; set; } = default!;
        public SyncStatus SyncStatus { get; set; } = SyncStatus.Sending;
    }
}