using BookStore.Core.Contracts;
using System.Text;

namespace BookStore.Core.Extensions
{
    public static class ModelExtensions
    {
        public static string GetInformation(this IBookModel book)
        {
            StringBuilder sb = new StringBuilder();

            string replaced = book.Title.Replace(" ", "-");
            replaced = replaced.Replace("'", string.Empty);

            sb.Append(replaced);

            return sb.ToString().TrimEnd();
        }
    }
}
