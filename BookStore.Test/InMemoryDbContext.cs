using BookStore.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Test
{
    /// <summary>
    /// Main class for setting up InMemoryDB
    /// </summary>
    public class InMemoryDbContext
    {
        private readonly SqliteConnection connection;
        private readonly DbContextOptions<ApplicationDbContext> dbContextOptions;

        /// <summary>
        /// Setting up InMemoryDbContext
        /// </summary>
        public InMemoryDbContext()
        {
            connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new ApplicationDbContext(dbContextOptions);

            context.Database.EnsureCreated();
        }

        /// <summary>
        /// Creating InMemoryDb Context
        /// </summary>
        /// <returns>ApplicationDbContext</returns>
        public ApplicationDbContext CreateContext() => new ApplicationDbContext(dbContextOptions);

        /// <summary>
        /// Disposing the context
        /// </summary>
        public void Dispose() => connection.Dispose();
    }
}
