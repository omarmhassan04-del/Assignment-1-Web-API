using System.ComponentModel.DataAnnotations;

namespace LibrarySystemApi.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
    }

    public class BookSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
    }

    public class CreateBookDto
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string ISBN { get; set; }

        [Required]
        public int AuthorId { get; set; }
    }

    public class UpdateBookDto
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string ISBN { get; set; }

        [Required]
        public int AuthorId { get; set; }
    }
}
