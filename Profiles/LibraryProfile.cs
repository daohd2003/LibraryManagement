using AutoMapper;
using LibraryManagement.DTOs;
using LibraryManagement.Models;

namespace LibraryManagement.Profiles
{
    public class LibraryProfile : Profile
    {
        public LibraryProfile()
        {
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.CategoryIds,
                opt => opt.MapFrom(src => src.BookCategories.Select(bc => bc.CategoryId))).ReverseMap();

            CreateMap<Book, BookDetailDto>()
                .IncludeBase<Book, BookDto>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.BookCategories.Select(bc => bc.Category)));

            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
