using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class List
    {
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private ICollection<Report> Reports { get; set; } = new List<Report>();
        private IDictionary<string, Customer> Customers { get; set; } = new Dictionary<string, Customer>();

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
            Customers = (await DbContext.Customers.GetAllAsync()).ToDictionary(x => x.Id);
            Reports = (await DbContext.Reports.GetAllAsync()).OrderByDescending(x => x.Date).ToList();
        }

        private async Task OnDeleteMenuItemClick(Report report)
        {
            await DbContext.Reports.DeleteAsync(report.Id);
            await FetchDataAsync();
        }
    }
}