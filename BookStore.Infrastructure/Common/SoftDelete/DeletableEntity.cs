namespace BookStore.Infrastructure.Common.SoftDelete
{
    internal class DeletableEntity : IDeletableEntity
    {
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
