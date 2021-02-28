using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafeWarehouseApp.Shared.Models
{
    public class Damage
    {
        public string Id { get; set; } = default!;
        [Required] public int Number { get; set; }
        [Required] public string? DamageTypeId { get; set; }
        public ICollection<RequiredMaterial> RequiredMaterials { get; set; } = new List<RequiredMaterial>();
        public ICollection<DamagePicture> Pictures { get; set; } = new List<DamagePicture>();
        
        public void UpdatePictureNumbers()
        {
            var currentIndex = 0;
            foreach (var picture in Pictures)
                picture.Number = ++currentIndex;
        }
    }
}