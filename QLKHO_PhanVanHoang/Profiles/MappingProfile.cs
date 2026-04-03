using AutoMapper;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.DTOs;

namespace QLKHO_PhanVanHoang.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            
            CreateMap<CreateReceivingVoucherDto, ReceivingVoucher>();
            CreateMap<CreateReceivingVoucherDetailDto, ReceivingVoucherDetail>();

            // Master data mappings
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<CreateWarehouseDto, Warehouse>();
            
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>();
            
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();

            // Delivery mappings
            CreateMap<DeliveryVoucher, DeliveryVoucherDto>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty));
            CreateMap<CreateDeliveryVoucherDto, DeliveryVoucher>();
            CreateMap<CreateDeliveryVoucherDetailDto, DeliveryVoucherDetail>();

            // Counting mappings
            CreateMap<CountingSheet, CountingSheetDto>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty));
            CreateMap<CreateCountingSheetDto, CountingSheet>();
            CreateMap<CreateCountingSheetDetailDto, CountingSheetDetail>();
        }
    }
}
