using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Services
{
    public class Store<T> where T:Document
    {
        private readonly string _name;
        private readonly Func<ValueTask<IJSObjectReference>> _moduleProvider;
        public Store(string name, Func<ValueTask<IJSObjectReference>> moduleProvider)
        {
            _name = name;
            _moduleProvider = moduleProvider;
        }

        public async Task<T> GetAsync(string id) => await (await _moduleProvider()).InvokeAsync<T>($"{_name}.get", id);
        public async Task<T> GetManyAsync(IEnumerable<string> ids) => await (await _moduleProvider()).InvokeAsync<T>($"{_name}.get", ids);
        public async Task<IEnumerable<T>> GetAllAsync() => await (await _moduleProvider()).InvokeAsync<IEnumerable<T>>($"{_name}.getAll");
        public async Task PutAsync(T document) => await (await _moduleProvider()).InvokeVoidAsync($"{_name}.put", document);
        public async Task DeleteAsync(string id) => await (await _moduleProvider()).InvokeVoidAsync($"{_name}.delete", id);
        public async Task DeleteManyAsync(IEnumerable<string> ids) => await (await _moduleProvider()).InvokeVoidAsync($"{_name}.delete", ids);
    }
}