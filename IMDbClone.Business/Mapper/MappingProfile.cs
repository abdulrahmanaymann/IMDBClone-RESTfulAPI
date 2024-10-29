using AutoMapper;
using IMDbClone.Core.DTOs.AuthDTOs;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.DTOs.ReviewDTOs;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.DTOs.WatchlistDTOs;
using IMDbClone.Core.Models;
using IMDbClone.Core.Utilities;

namespace IMDbClone.Business.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieDTO>()
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastList))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews ?? new List<Review>()))
                .ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src.Ratings ?? new List<Rating>()))
                .ReverseMap();

            CreateMap<Movie, CreateMovieDTO>()
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastList))
                .ReverseMap();

            CreateMap<Movie, UpdateMovieDTO>()
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastList))
                .ReverseMap();

            CreateMap<Movie, MovieSummaryDTO>()
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Ratings.Count != 0 ? Math.Round(src.Ratings.Average(r => r.Score), 1) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count));


            CreateMap<Rating, RatingDTO>().ReverseMap();
            CreateMap<Rating, CreateRatingDTO>().ReverseMap();
            CreateMap<Rating, UpdateRatingDTO>().ReverseMap();

            CreateMap<Review, ReviewDTO>().ReverseMap();
            CreateMap<Review, CreateReviewDTO>().ReverseMap();
            CreateMap<Review, UpdateReviewDTO>().ReverseMap();

            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<ApplicationUser, LoginResponseDTO>().ReverseMap();
            CreateMap<RegisterationRequestDTO, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, UserProfileDTO>().ReverseMap();

            CreateMap<Watchlist, WatchlistDTO>()
               .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
               .ReverseMap();
            CreateMap<Watchlist, CreateWatchlistDTO>().ReverseMap();

            CreateMap<PaginatedResult<Rating>, PaginatedResult<RatingDTO>>()
                        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<PaginatedResult<Review>, PaginatedResult<ReviewDTO>>()
                    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        }
    }
}