using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Materials
{
    partial class List
    {
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private ICollection<Material> Materials { get; set; } = new List<Material>();

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
            Materials = (await DbContext.Materials.GetAllAsync()).OrderBy(x => x.Name).ToList();
        }

        private async Task OnDeleteMenuItemClick(Material material)
        {
            await DbContext.Materials.DeleteAsync(material.Id);
            await FetchDataAsync();
        }
    }
}