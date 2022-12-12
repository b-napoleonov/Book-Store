namespace BookStore.Infrastructure.Common.SoftDeleteBaseClass
{
    /// <summary>
    /// SoftDelete base interface
    /// </summary>
    public interface IDeletableEntity
    {
        bool IsDeleted { get; set; }

        DateTime? DeletedOn { get; set; }
    }
}
