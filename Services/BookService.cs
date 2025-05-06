using AutoMapper;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository repository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _bookRepository = repository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<BookDetailDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDetailDto>>(books);
        }

        public async Task<BookDetailDto?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return _mapper.Map<BookDetailDto?>(book);
        }

        public async Task<BookDetailDto> AddBookAsync(BookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);

            // Validate categories exist
            foreach (var categoryId in bookDto.CategoryIds)
            {
                if (!await _categoryRepository.ExistsAsync(categoryId))
                {
                    throw new KeyNotFoundException($"Category with ID {categoryId} not found");
                }

                book.BookCategories.Add(new BookCategory
                {
                    BookId = book.Id,
                    CategoryId = categoryId
                });
            }

            var addedBook = await _bookRepository.AddAsync(book);
            return _mapper.Map<BookDetailDto>(addedBook);
        }

        public async Task UpdateBookAsync(int id, BookDto bookDto)
        {
            if (id != bookDto.Id)
            {
                throw new ArgumentException("ID mismatch");
            }

            if (!await _bookRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException("Book not found");
            }

            // Validate categories exist
            foreach (var categoryId in bookDto.CategoryIds)
            {
                if (!await _categoryRepository.ExistsAsync(categoryId))
                {
                    throw new KeyNotFoundException($"Category with ID {categoryId} not found");
                }
            }

            var book = _mapper.Map<Book>(bookDto);
            await _bookRepository.UpdateAsync(book);
        }

        public async Task DeleteBookAsync(int id)
        {
            if (!await _bookRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException("Book not found");
            }

            await _bookRepository.DeleteAsync(id);
        }
    }
}
