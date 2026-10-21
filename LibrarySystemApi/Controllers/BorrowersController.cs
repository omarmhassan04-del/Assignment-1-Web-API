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
    public class BorrowersController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BorrowersController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowerDto>>> GetAll()
        {
            var borrowers = await _context.Borrowers
                .Include(b => b.Loans)
                    .ThenInclude(l => l.Book)
                .AsNoTracking()
                .Select(b => new BorrowerDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    MembershipDate = b.MembershipDate,
                    Loans = b.Loans.Select(l => new LoanDto
                    {
                        BookId = l.BookId,
                        BookTitle = l.Book != null ? l.Book.Title : null,
                        BorrowerId = l.BorrowerId,
                        BorrowerName = b.Name,
                        LoanDate = l.LoanDate,
                        ReturnDate = l.ReturnDate
                    }).ToList()
                })
                .ToListAsync();

            return Ok(borrowers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowerDto>> GetById(int id)
        {
            var borrower = await _context.Borrowers
                .Include(b => b.Loans)
                    .ThenInclude(l => l.Book)
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BorrowerDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    MembershipDate = b.MembershipDate,
                    Loans = b.Loans.Select(l => new LoanDto
                    {
                        BookId = l.BookId,
                        BookTitle = l.Book != null ? l.Book.Title : null,
                        BorrowerId = l.BorrowerId,
                        BorrowerName = b.Name,
                        LoanDate = l.LoanDate,
                        ReturnDate = l.ReturnDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (borrower == null)
            {
                return NotFound();
            }

            return Ok(borrower);
        }

        [HttpPost]
        public async Task<ActionResult<BorrowerDto>> Create([FromBody] CreateBorrowerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var borrower = new Borrower
            {
                Name = dto.Name,
                MembershipDate = dto.MembershipDate
            };

            await _context.Borrowers.AddAsync(borrower);
            await _context.SaveChangesAsync();

            var result = new BorrowerDto
            {
                Id = borrower.Id,
                Name = borrower.Name,
                MembershipDate = borrower.MembershipDate,
                Loans = new List<LoanDto>()
            };

            return CreatedAtAction(nameof(GetById), new { id = borrower.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBorrowerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }

            borrower.Name = dto.Name;
            borrower.MembershipDate = dto.MembershipDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }

            _context.Borrowers.Remove(borrower);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
