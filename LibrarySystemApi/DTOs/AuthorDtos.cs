using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystemApi.DTOs
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public List<BookSummaryDto> Books { get; set; } = new List<BookSummaryDto>();
    }

    public class CreateAuthorDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
    }

    public class UpdateAuthorDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
    }
}
