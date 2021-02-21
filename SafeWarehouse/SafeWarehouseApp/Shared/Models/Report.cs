using System;
using System.Collections.Generic;

namespace SafeWarehouseApp.Shared.Models
{
    public class Report : Document
    {
        public string? CustomerId { get; set; } = default!;
        public DateTime Date { get; set; }
        public DateTime? NextExaminationBefore { get; set; }
        public string? Remarks { get; set; }
        public string OriginalSchematicId { get; set; } = default!;
        public string SchematicPhotoId { get; set; } = default!;
        public string? PhotoId { get; set; }
        public IList<Location> Locations { get; set; } = new List<Location>();

        public void UpdateLocationNumbers()
        {
            var currentIndex = 0;
            foreach (var location in Locations)
                location.Number = ++currentIndex;
        }
    }
}