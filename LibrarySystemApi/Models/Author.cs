using System;
using System.Collections.Generic;

namespace LibrarySystemApi.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public ICollection<Book> Books { get; set; } = new HashSet<Book>();
    }
}
