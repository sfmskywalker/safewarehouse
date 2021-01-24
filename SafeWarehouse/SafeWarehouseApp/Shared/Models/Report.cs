using System;
using System.Collections.Generic;

namespace SafeWarehouseApp.Shared.Models
{
    public class Report : Document
    {
        public string Title { get; set; } = default!;
        public string? Location { get; set; }
        public DateTime Date { get; set; }
        public File File { get; set; } = new();
        public ICollection<Damage> Damages { get; set; } = new List<Damage>();
    }
}