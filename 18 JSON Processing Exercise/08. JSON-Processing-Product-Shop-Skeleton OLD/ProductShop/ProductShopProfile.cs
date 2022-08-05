using AutoMapper;
using ProductShop.DTOs.Category;
using ProductShop.DTOs.CategoryProduct;
using ProductShop.DTOs.Product;
using ProductShop.DTOs.User;

using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<ImportUserDto, User>();
            this.CreateMap<ImportProductDto, Product>();
            this.CreateMap<ImportCategoryDto, Category>();
            this.CreateMap<ImportCategoryProductsDto, CategoryProduct>();

            this.CreateMap<Product, ExportInRangeProductDto>()
                .ForMember(destination => destination.SellerFullName,
                memberoptions => memberoptions
                .MapFrom(source => $"{source.Seller.FirstName} {source.Seller.LastName}"));

            this.CreateMap<Product, ExportUserSoldProductsDto>()
                .ForMember(d => d.BuyerFirstName, mo => mo.MapFrom(s => s.Buyer.FirstName))
                .ForMember(d => d.BuyerLastName, mo => mo.MapFrom(mo => mo.Buyer.LastName));
            this.CreateMap<User, ExportUserWithSoldProductsDto>()
                .ForMember(d => d.SoldProducts, mo => mo.MapFrom(s => s.ProductsSold
                .Where(p => p.BuyerId.HasValue)));

            this.CreateMap<Product, ExportSoldProductShortInfoDto>();
            this.CreateMap<User, ExportSoldProductsFullInfoDto>()
                .ForMember(d => d.SoldProducts, mo => mo
                    .MapFrom(s => s.ProductsSold.Where(b => b.BuyerId.HasValue)));
            this.CreateMap<User, ExportUsersWithFullProductInfoDto>()
                .ForMember(d => d.SoldProductsInfo, mo => mo.MapFrom(s => s));
        }
    }
}
