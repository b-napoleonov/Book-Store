using BookStore.Core.Contracts;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BookStoreServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPublisherService, PublisherService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(DeletableEntityRepository<>));

            return services;
        }
    }
}
