using System.Collections.Generic;

namespace SafeWarehouseApp.Shared.Models
{
    public class Location : Document
    {
        public int Number { get; set; }
        public string? Description { get; set; }
        public ICollection<Damage> Damages { get; set; } = new List<Damage>();
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}