﻿using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Author
    {
        public Author()
        {
            this.AuthorBooks = new HashSet<AuthorBook>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; }
    }
}
