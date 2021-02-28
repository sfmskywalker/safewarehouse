using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Customers
{
    partial class Edit
    {
        [Parameter] public string? Id { private get; set; }
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        private Customer Customer { get; set; } = new();
        private EditContext CustomerContext { get; set; } = default!;
        private bool HasRendered { get; set; }

        protected override void OnInitialized()
        {
            CustomerContext = new EditContext(Customer);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (HasRendered) 
                await LoadCustomerAsync();
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadCustomerAsync();
                HasRendered = true;
                StateHasChanged();
            }
        }
        
        private async Task LoadCustomerAsync()
        {
            if (Id is null or "")
                return;
            
            var customer = await DbContext.Customers.GetAsync(Id);
            SetCustomer(customer);
        }
        
        private void SetCustomer(Customer customer)
        {
            CustomerContext = new EditContext(customer);
            Customer = customer;
        }

        private async Task SaveChangesAsync() => await DbContext.Customers.PutAsync(Customer);

        private async Task OnValidSubmit()
        {
            if (Customer.Id is null or "")
            {
                Customer.Id = Guid.NewGuid().ToString();
            }

            await SaveChangesAsync();
            NavigationManager.NavigateTo("customers");
        }
    }
}