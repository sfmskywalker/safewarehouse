using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafeWarehouseApp.Shared.Models
{
    public class Damage
    {
        public string Id { get; set; } = default!;
        [Required] public string Number { get; set; } = default!;
        [Required] public string DamageTypeId { get; set; } = default!;
        public IDictionary<string, int> RequiredMaterials { get; set; } = new Dictionary<string, int>();
        public ICollection<DamagePicture> Pictures { get; set; } = new List<DamagePicture>();
    }
}