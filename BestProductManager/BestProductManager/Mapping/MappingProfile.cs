using AutoMapper;
using BestProductManager.Api.Dtos.Products;
using BestProductManager.Api.Entities;

namespace BestProductManager.Api.Mapping
{
    /// <summary>
    /// Cấu hình ánh xạ giữa Entity và DTO bằng AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Khởi tạo cấu hình ánh xạ cho Product.
        /// </summary>
        public MappingProfile()
        {
            // Ánh xạ hai chiều giữa Product và ProductDto.
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, UpdProductDto>().ReverseMap();
        }
    }
}
