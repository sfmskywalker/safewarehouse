using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Customers
{
    partial class List
    {
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private ICollection<Customer> Customers { get; set; } = new List<Customer>();

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
            Customers = (await DbContext.Customers.GetAllAsync()).OrderBy(x => x.CompanyName).ToList();
        }

        private async Task OnDeleteMenuItemClick(Customer customer)
        {
            await DbContext.Customers.DeleteAsync(customer.Id);
            await FetchDataAsync();
        }
    }
}