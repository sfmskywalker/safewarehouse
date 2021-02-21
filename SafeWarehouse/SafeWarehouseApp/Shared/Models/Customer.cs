namespace SafeWarehouseApp.Shared.Models
{
    public class Customer : Document
    {
        public string CompanyName { get; set; } = default!;
        public string ContactName { get; set; }= default!;
        public string Email { get; set; }= default!;
        public string City { get; set; }= default!;
        public string Address { get; set; }= default!;
    }
}