using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystemApi.Data;
using LibrarySystemApi.DTOs;
using LibrarySystemApi.Models;

namespace LibrarySystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LoansController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Borrower)
                .AsNoTracking()
                .Select(l => new LoanDto
                {
                    BookId = l.BookId,
                    BookTitle = l.Book != null ? l.Book.Title : null,
                    BorrowerId = l.BorrowerId,
                    BorrowerName = l.Borrower != null ? l.Borrower.Name : null,
                    LoanDate = l.LoanDate,
                    ReturnDate = l.ReturnDate
                })
                .ToListAsync();

            return Ok(loans);
        }

        [HttpGet("{bookId}/{borrowerId}/{loanDate}")]
        public async Task<ActionResult<LoanDto>> GetById(int bookId, int borrowerId, DateTime loanDate)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Borrower)
                .AsNoTracking()
                .Where(l => l.BookId == bookId && l.BorrowerId == borrowerId && l.LoanDate == loanDate)
                .Select(l => new LoanDto
                {
                    BookId = l.BookId,
                    BookTitle = l.Book != null ? l.Book.Title : null,
                    BorrowerId = l.BorrowerId,
                    BorrowerName = l.Borrower != null ? l.Borrower.Name : null,
                    LoanDate = l.LoanDate,
                    ReturnDate = l.ReturnDate
                })
                .FirstOrDefaultAsync();

            if (loan == null)
            {
                return NotFound();
            }

            return Ok(loan);
        }

        [HttpPost]
        public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookExists = await _context.Books.AnyAsync(b => b.Id == dto.BookId);
            if (!bookExists)
            {
                return BadRequest($"Book with ID {dto.BookId} does not exist.");
            }

            var borrowerExists = await _context.Borrowers.AnyAsync(b => b.Id == dto.BorrowerId);
            if (!borrowerExists)
            {
                return BadRequest($"Borrower with ID {dto.BorrowerId} does not exist.");
            }

            var loanExists = await _context.Loans.AnyAsync(l =>
                l.BookId == dto.BookId &&
                l.BorrowerId == dto.BorrowerId &&
                l.LoanDate == dto.LoanDate);

            if (loanExists)
            {
                return Conflict("A loan record with the same Book, Borrower, and Loan Date already exists.");
            }

            var loan = new Loan
            {
                BookId = dto.BookId,
                BorrowerId = dto.BorrowerId,
                LoanDate = dto.LoanDate,
                ReturnDate = dto.ReturnDate
            };

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            await _context.Entry(loan).Reference(l => l.Book).LoadAsync();
            await _context.Entry(loan).Reference(l => l.Borrower).LoadAsync();

            var result = new LoanDto
            {
                BookId = loan.BookId,
                BookTitle = loan.Book?.Title,
                BorrowerId = loan.BorrowerId,
                BorrowerName = loan.Borrower?.Name,
                LoanDate = loan.LoanDate,
                ReturnDate = loan.ReturnDate
            };

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    bookId = loan.BookId,
                    borrowerId = loan.BorrowerId,
                    loanDate = loan.LoanDate.ToString("o")
                },
                result);
        }

        [HttpPut("{bookId}/{borrowerId}/{loanDate}")]
        public async Task<IActionResult> Update(int bookId, int borrowerId, DateTime loanDate, [FromBody] UpdateLoanDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loan = await _context.Loans.FindAsync(bookId, borrowerId, loanDate);
            if (loan == null)
            {
                return NotFound();
            }

            loan.ReturnDate = dto.ReturnDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{bookId}/{borrowerId}/{loanDate}")]
        public async Task<IActionResult> Delete(int bookId, int borrowerId, DateTime loanDate)
        {
            var loan = await _context.Loans.FindAsync(bookId, borrowerId, loanDate);
            if (loan == null)
            {
                return NotFound();
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
