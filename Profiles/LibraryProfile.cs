using AutoMapper;
using LibraryManagement.DTOs.Request;
using LibraryManagement.DTOs.Response;
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
                    opt => opt.MapFrom(src => src.BookCategories.Select(bc => bc.Category))).ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Category, CreateCategoryDto>()
                .ReverseMap();

            CreateMap<Category, UpdateCategoryDto>()
                .ReverseMap();

            CreateMap<BorrowedBook, BorrowedBookDto>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<BorrowedBook, BorrowedBookDetailDto>()
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        }
    }
}
