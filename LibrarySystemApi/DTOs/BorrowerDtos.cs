using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystemApi.DTOs
{
    public class BorrowerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime MembershipDate { get; set; }
        public List<LoanDto> Loans { get; set; } = new List<LoanDto>();
    }

    public class CreateBorrowerDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime MembershipDate { get; set; }
    }

    public class UpdateBorrowerDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime MembershipDate { get; set; }
    }
}
