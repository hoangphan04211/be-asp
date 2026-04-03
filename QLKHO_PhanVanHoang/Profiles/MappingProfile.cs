using AutoMapper;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.DTOs;

namespace QLKHO_PhanVanHoang.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
            CreateMap<CreateProductDto, Product>();
            
            // Receiving mappings
            CreateMap<ReceivingVoucher, ReceivingVoucherDto>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Details.Sum(d => d.Quantity * (d.UnitPrice ?? 0))));
            
            CreateMap<ReceivingVoucherDetail, ReceivingVoucherDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product != null ? src.Product.SkuCode : string.Empty));
            
            CreateMap<CreateReceivingVoucherDto, ReceivingVoucher>()
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
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
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Details.Sum(d => d.Quantity * (d.SellingPrice ?? 0))));
            
            CreateMap<DeliveryVoucherDetail, DeliveryVoucherDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product != null ? src.Product.SkuCode : string.Empty));
            
            CreateMap<CreateDeliveryVoucherDto, DeliveryVoucher>();
            CreateMap<CreateDeliveryVoucherDetailDto, DeliveryVoucherDetail>();

            // Counting mappings
            CreateMap<CountingSheet, CountingSheetDto>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty));
            
            CreateMap<CountingSheetDetail, CountingSheetDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product != null ? src.Product.SkuCode : string.Empty))
                .ForMember(dest => dest.PhysicalQuantity, opt => opt.MapFrom(src => src.ActualQuantity));

            CreateMap<CreateCountingSheetDto, CountingSheet>();
            CreateMap<CreateCountingSheetDetailDto, CountingSheetDetail>()
                .ForMember(dest => dest.ActualQuantity, opt => opt.MapFrom(src => src.PhysicalQuantity));
        }
    }
}
