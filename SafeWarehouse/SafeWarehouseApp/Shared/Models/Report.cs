using System;
using System.Collections.Generic;
using SafeWarehouseApp.Shared.Extensions;

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
        public IList<Location> Locations { get; set; } = new List<Location>();

        public string GetLocationNotation(Location location)
        {
            var index = Locations.IndexOf(location);
            var letter = Helpers.IntToLetters(index + 1);
            var damageCount = location.Damages.Count;

            return damageCount == 0 ? letter : $"{letter} ({damageCount})";
        }
    }
}