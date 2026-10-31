using AutoMapper;
using Order.Application.DataContract.Request.Client;
using Order.Application.DataContract.Request.Order;
using Order.Application.DataContract.Request.Product;
using Order.Application.DataContract.Request.User;
using Order.Application.DataContract.Response.Client;
using Order.Application.DataContract.Response.Order;
using Order.Application.DataContract.Response.Product;
using Order.Application.DataContract.Response.User;
using Order.Domain.Models;

namespace Order.Application.Mapper
{
    public class Core : Profile
    {
        public Core()
        {
            ClientMap();
        }

        private void ClientMap()
        {
            CreateMap<CreateClientRequest, ClientModel>();
            CreateMap<UpdateClientRequest, ClientModel>();

            CreateMap<ClientModel, clientResponse>();

            CreateMap<CreateUserRequest, UserModel>()
                .ForMember(target => target.PasswordHash, opt => opt.MapFrom(source => source.Password));
            CreateMap<UserModel, UserResponse>();

            CreateMap<CreateOrderRequest, OrderModel>()
                .ForPath(target => target.Client.Id, opt => opt.MapFrom(source => source.ClientId))
                .ForPath(target => target.User.Id, opt => opt.MapFrom(source => source.UserId));

            CreateMap<OrderModel, OrderResponse>()
                .ForMember(target => target.ClientId, opt => opt.MapFrom(source => source.Client.Id))
                .ForMember(target => target.ClientName, opt => opt.MapFrom(source => source.Client.Name))
                .ForMember(target => target.UserId, opt => opt.MapFrom(source => source.User.Id))
                .ForMember(target => target.UserName, opt => opt.MapFrom(source => source.User.Name))
                .ForMember(target => target.TotalAmount,
                           opt => opt.MapFrom(source => source.Items == null ? 0m : source.Items.Sum(i => i.TotalAmount)));

            CreateMap<CreateOrderItemRequest, OrderItemModel>()
                .ForPath(target => target.Product.Id, opt => opt.MapFrom(source => source.ProductId));

            CreateMap<OrderItemModel, OrderItemResponse>()
                .ForMember(target => target.ProductId, opt => opt.MapFrom(source => source.Product.Id))
                .ForMember(target => target.ProductDescription, opt => opt.MapFrom(source => source.Product.Description));

            CreateMap<CreateProductRequest, ProductModel>();
            CreateMap<ProductModel, ProductResponse>();

        }
    }
}
