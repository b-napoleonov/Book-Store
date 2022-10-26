namespace BookStore.Infrastructure.Common.SoftDeleteBaseClass
{
    public interface IDeletableEntity
    {
        bool IsDeleted { get; set; }

        DateTime? DeletedOn { get; set; }
    }
}
