namespace BookStore.Infrastructure.Common.SoftDeleteBaseClass
{
    /// <summary>
    /// SoftDelete base class
    /// </summary>
    internal class DeletableEntity : IDeletableEntity
    {
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
