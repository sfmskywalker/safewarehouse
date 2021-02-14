using AutoMapper;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Location, Location>();
        }
    }
}