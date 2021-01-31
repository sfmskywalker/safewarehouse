using System.Collections.Generic;

namespace SafeWarehouseApp.Shared.Models
{
    public class Damage : Document
    {
        public int Number { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? DamageTypeId { get; set; } = default!;
        public string? MaterialId { get; set; }
        public int? MaterialQuantity { get; set; }
        public ICollection<DamageDetail> Details = new List<DamageDetail>();
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}