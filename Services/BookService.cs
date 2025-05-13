using AutoMapper;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Hubs;
using LibraryManagement.Models;
using LibraryManagement.Repositories;
using Microsoft.AspNetCore.SignalR;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryManagement.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<LibraryHub> _hubContext;

        public BookService(IBookRepository repository, IMapper mapper, ICategoryRepository categoryRepository, IHubContext<LibraryHub> hubContext)
        {
            _bookRepository = repository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _hubContext = hubContext;
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
            await _hubContext.Clients.All.SendAsync("BookAdded", book);
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

        public async Task<IEnumerable<BookDetailDto>> GetPagedBooksAsync(int pageNumber, int pageSize)
        {
            var books = await _bookRepository.GetPagedBooksAsync(pageNumber, pageSize);

            return _mapper.Map<IEnumerable<BookDetailDto>>(books);
        }

        public async Task<IEnumerable<BookDetailDto>> SearchBooksPagedAsync(string keyword, int lastId, int pageSize)
        {
            var books = await _bookRepository.SearchBooksPagedAsync(keyword, lastId, pageSize);
            return _mapper.Map<IEnumerable<BookDetailDto>>(books);
        }
    }
}
