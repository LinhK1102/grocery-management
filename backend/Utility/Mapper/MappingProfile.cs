using BusinessObjects.Entities;
using AutoMapper;
using BusinessObjects.DTOs; // ← nếu DTO của bạn nằm đây

namespace Utility.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateSupplierDto, Supplier>();
            CreateMap<Supplier, UpdateSupplierDto>();
        }
    }
}
