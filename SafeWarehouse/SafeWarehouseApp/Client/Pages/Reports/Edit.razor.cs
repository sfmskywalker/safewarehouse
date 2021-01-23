using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class Edit
    {
        [Parameter] public string Id { private get; set; } = default!;
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private Report Report { get; set; } = new();
        private bool HasRendered { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if(HasRendered)
                Report = await DbContext.Reports.GetAsync(Id);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                Report = await DbContext.Reports.GetAsync(Id);
                HasRendered = true;
                StateHasChanged();
            }
        }
    }
}