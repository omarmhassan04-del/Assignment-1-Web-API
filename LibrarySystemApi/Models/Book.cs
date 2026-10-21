using System.Collections.Generic;

namespace LibrarySystemApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public ICollection<Loan> Loans { get; set; } = new HashSet<Loan>();
    }
}
