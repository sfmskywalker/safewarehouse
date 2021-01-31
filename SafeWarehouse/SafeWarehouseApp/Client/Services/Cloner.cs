using AutoMapper;

namespace SafeWarehouseApp.Client.Services
{
    public class Cloner
    {
        private readonly IMapper _mapper;
        public Cloner(IMapper mapper) => _mapper = mapper;
        public T Clone<T>(T value) => _mapper.Map<T>(value);
    }
}