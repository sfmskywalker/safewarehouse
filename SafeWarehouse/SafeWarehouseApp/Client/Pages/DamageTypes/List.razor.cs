using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.DamageTypes
{
    partial class List
    {
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private ICollection<DamageType> DamageTypes { get; set; } = new List<DamageType>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDataAsync();
                StateHasChanged();
            }
        }

        private async Task FetchDataAsync()
        {
            DamageTypes = (await DbContext.DamageTypes.GetAllAsync()).OrderBy(x => x.Title).ToList();
        }

        private async Task OnDeleteMenuItemClick(DamageType damageType)
        {
            await DbContext.DamageTypes.DeleteAsync(damageType.Id);
            await FetchDataAsync();
        }
    }
}