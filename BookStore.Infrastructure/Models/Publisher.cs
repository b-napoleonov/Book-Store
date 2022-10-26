﻿using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Publisher : IDeletableEntity
    {
        public Publisher()
        {
            this.Books = new HashSet<Book>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        public string Phone { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string URL { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
