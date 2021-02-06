using System;
using System.Collections.Generic;

namespace SafeWarehouseApp.Shared.Models
{
    public class Report : Document
    {
        public string Title { get; set; } = default!;
        public string? Location { get; set; }
        public DateTime Date { get; set; }
        public string? City { get; set; }
        public string? Remarks { get; set; }
        public File Schematic { get; set; } = new();
        public File? Photo { get; set; }
        public ICollection<Damage> Damages { get; set; } = new List<Damage>();
        
    }
}