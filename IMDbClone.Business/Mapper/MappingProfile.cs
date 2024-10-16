﻿using AutoMapper;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.DTOs.ReviewDTOs;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.Entities;

namespace IMDbClone.Business.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieDTO>()
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastList))
                .ReverseMap();

            CreateMap<Movie, CreateMovieDTO>()
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastList))
                .ReverseMap();

            CreateMap<Movie, UpdateMovieDTO>()
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastList))
                .ReverseMap();

            CreateMap<Rating, RatingDTO>().ReverseMap();
            CreateMap<Rating, CreateRatingDTO>().ReverseMap();
            CreateMap<Rating, UpdateRatingDTO>().ReverseMap();

            CreateMap<Review, ReviewDTO>().ReverseMap();
            CreateMap<Review, CreateReviewDTO>().ReverseMap();
            CreateMap<Review, UpdateReviewDTO>().ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
