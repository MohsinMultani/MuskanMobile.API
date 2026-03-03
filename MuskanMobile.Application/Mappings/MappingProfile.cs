using AutoMapper;
using MuskanMobile.Domain.Entities;
using MuskanMobile.Application.DTOs;
using System;
using MuskanMobile.Application.DTOs.Customer;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
            .ForMember(dest => dest.SupplierName,
                opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : null))
            .ForMember(dest => dest.TaxRateName,
                opt => opt.MapFrom(src => src.TaxRate != null ? src.TaxRate.TaxName : null))
            .ForMember(dest => dest.TaxPercentage,
                opt => opt.MapFrom(src => src.TaxRate != null ? src.TaxRate.Rate : (decimal?)null));

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<Product, ProductDropdownDto>();

        // Category mappings (new)
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Suppliers
        CreateMap<Supplier, SupplierDto>().ReverseMap();
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<UpdateSupplierDto, Supplier>();
        CreateMap<Supplier, SupplierDropdownDto>();

        //TaxRate
        CreateMap<TaxRate, TaxRateDto>().ReverseMap();
        CreateMap<CreateTaxRateDto, TaxRate>();
        CreateMap<UpdateTaxRateDto, TaxRate>();
        CreateMap<TaxRate, TaxRateDropdownDto>();

        // Customer
        CreateMap<Customer, CustomerDto>().ReverseMap();
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();
        CreateMap<Customer, CustomerDropdownDto>();

        // SalesOrder mappings
        CreateMap<SalesOrder, SalesOrderDto>()
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Customer != null ? src.Customer.CustomerName : null));

        CreateMap<SalesItem, SalesItemDto>()
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : null));

        CreateMap<CreateSalesOrderDto, SalesOrder>();
        CreateMap<CreateSalesItemDto, SalesItem>();
        CreateMap<UpdateSalesOrderDto, SalesOrder>();

        // PurchaseOrder
        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .ForMember(dest => dest.SupplierName,
                opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : null));

        CreateMap<PurchaseItem, PurchaseItemDto>()
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : null));

        CreateMap<CreatePurchaseOrderDto, PurchaseOrder>();
        CreateMap<CreatePurchaseItemDto, PurchaseItem>();
        CreateMap<UpdatePurchaseOrderDto, PurchaseOrder>();
    }
}
