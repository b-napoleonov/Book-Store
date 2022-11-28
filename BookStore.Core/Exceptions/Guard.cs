namespace BookStore.Core.Exceptions
{
	public class Guard : IGuard
	{
		public void AgainsNull<T>(T value, string? errorMessage = null)
		{
            throw new NotImplementedException();

            //if (value == null)
            //{
            //exception type
            //	var exception = errorMessage == null ? new ArgumentException() : new ArgumentException(errorMessage);

            //	throw exception;
            //}
        }
	}
}
