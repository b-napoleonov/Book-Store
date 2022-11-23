using BookStore.Core.Models.Book;
using BookStore.Core.Models.Rating;
using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync();

        Task<DetailsBookViewModel> GetBookAsync(Guid bookId);

        Task AddBookAsync(AddBookViewModel model);

        Task<IEnumerable<AllBooksViewModel>> GetBooksByCategoryAsync(string categoryName);

        Task<IEnumerable<AllBooksViewModel>> GetBooksByAuthorAsync(string authorName);

        Task<IEnumerable<AllBooksViewModel>> GetBooksByPublisherAsync(string publisherName);

        Task<Book> GetBookByIdAsync(Guid bookId);

        Task<double> GetBookRating(Guid bookId);

        Task<IEnumerable<DetailsReviewViewModel>> GetBookReviewsAsync(Guid bookId);

        Task<DetailsRatingViewModel> GetBookRatingDetailsAsync(Guid bookId);

        Task RemoveBook(Guid bookId);

        Task<EditBookViewModel> GetBookDataForEditAsync(Guid bookId);

        Task EditBookAsync(EditBookViewModel model, Guid bookId);

        Task<IEnumerable<HomeBookViewModel>> GetLastThreeBooksAsync();
    }
}
