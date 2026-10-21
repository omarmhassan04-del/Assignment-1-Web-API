using System;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystemApi.DTOs
{
    public class LoanDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int BorrowerId { get; set; }
        public string BorrowerName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class CreateLoanDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int BorrowerId { get; set; }

        [Required]
        public DateTime LoanDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }

    public class UpdateLoanDto
    {
        public DateTime? ReturnDate { get; set; }
    }
}
