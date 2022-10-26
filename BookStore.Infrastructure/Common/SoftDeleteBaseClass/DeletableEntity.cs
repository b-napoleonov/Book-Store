namespace BookStore.Infrastructure.Common.SoftDeleteBaseClass
{
    internal class DeletableEntity : IDeletableEntity
    {
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
