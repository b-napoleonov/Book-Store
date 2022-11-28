namespace BookStore.Core.Exceptions
{
	public interface IGuard
	{
		void AgainsNull<T>(T value, string? errorMessage = null);
	}
}
