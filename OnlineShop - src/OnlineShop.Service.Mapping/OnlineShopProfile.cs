using AutoMapper;
using OnlineShop.Models;
using OnlineShop.Web.ViewModels.Administrator.Category;
using OnlineShop.Web.ViewModels.Administrator.Home;
using OnlineShop.Web.ViewModels.Administrator.Product;
using OnlineShop.Web.ViewModels.Administrator.SubCategory;
using OnlineShop.Web.ViewModels.Home;
using OnlineShop.Web.ViewModels.Order;
using OnlineShop.Web.ViewModels.ShoppingCart;
using OnlineShop.Web.ViewModels.Product;
using System.Linq;

namespace OnlineShop.Service.Mapping
{
    public class OnlineShopProfile : Profile
    {
        public OnlineShopProfile()
        {
            this.CreateMap<ParentCategory, ParentCategoryViewModel>();

            this.CreateMap<SubCategory, SubCategoryViewModel>();
            this.CreateMap<SubCategory, EditSubCategoryViewModel>();

            this.CreateMap<CreateProductViewModel, Product>();

            this.CreateMap<Product, EditProductViewModel>();
            this.CreateMap<EditProductViewModel, Product>();
            this.CreateMap<Address, OrderAdressViewModel>();
            this.CreateMap<Order, CreateOrderViewModel>();

            this.CreateMap<Order, ConfirmOrderViewModel>()
                            .ForMember(x => x.TotalPrice, y => y.MapFrom(src => src.TotalPrice));

            this.CreateMap<Product, DetailProductViewModel>()
                .ForMember(p => p.Images, y => y.MapFrom(src => src.Images.Select(x => x.ImageUrl)));

            this.CreateMap<Product, ProductViewModel>()
                .ForMember(x => x.ImageUrl, y => y.MapFrom(src => src.Images.FirstOrDefault().ImageUrl));


            this.CreateMap<Product, IndexProductViewModel>()
                .ForMember(x => x.ImageUrl, y => y.MapFrom(src => src.Images.FirstOrDefault().ImageUrl));
            
            this.CreateMap<Order, IndexProcessedOrdersViewModel>()
                .ForMember(x => x.PaymentStatus, y => y.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(x => x.PaymentType, y => y.MapFrom(s => s.PaymentType.ToString()));

            this.CreateMap<Order, IndexUnprocessedOrdersViewModel>()
                .ForMember(x => x.PaymentStatus, y => y.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(x => x.PaymentType, y => y.MapFrom(s => s.PaymentType.ToString()));

        }
    }
}
