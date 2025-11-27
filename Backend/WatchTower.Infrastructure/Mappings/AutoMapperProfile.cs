using AutoMapper;
namespace WatchTower.Infrastructure.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<User, UserResponse>();
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Core.Enums.UserRole.Visitor));

        // CelestialBody mappings
        CreateMap<CelestialBody, CelestialBodyResponse>();
        CreateMap<CelestialBody, CelestialBodyDetailResponse>();
        CreateMap<CelestialBodyCreateRequest, CelestialBody>();
        
        // Discovery mappings
        CreateMap<Discovery, DiscoveryResponse>();
        CreateMap<Discovery, DiscoveryDetailResponse>();
        CreateMap<DiscoveryCreateRequest, Discovery>();
        
        // Article mappings
        CreateMap<Article, ArticleResponse>();
        CreateMap<Article, ArticleDetailResponse>();
        CreateMap<ArticleCreateRequest, Article>();
        
        // Favorite mappings
        CreateMap<Favorite, FavoriteResponse>()
            .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => 
                src.CelestialBodyId != null ? "CelestialBody" :
                src.ArticleId != null ? "Article" : "Discovery"))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => 
                src.CelestialBody?.Name ?? src.Article?.Title ?? src.Discovery?.Title ?? "Unknown"))
            .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => 
                src.CelestialBodyId ?? src.ArticleId ?? src.DiscoveryId));
    }
}