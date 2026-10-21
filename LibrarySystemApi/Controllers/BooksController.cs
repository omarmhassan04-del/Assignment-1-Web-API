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
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .AsNoTracking()
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? b.Author.Name : null
                })
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? b.Author.Name : null
                })
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorExists = await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId);
            if (!authorExists)
            {
                return BadRequest($"Author with ID {dto.AuthorId} does not exist.");
            }

            var book = new Book
            {
                Title = dto.Title,
                ISBN = dto.ISBN,
                AuthorId = dto.AuthorId
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            await _context.Entry(book).Reference(b => b.Author).LoadAsync();

            var result = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name
            };

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var authorExists = await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId);
            if (!authorExists)
            {
                return BadRequest($"Author with ID {dto.AuthorId} does not exist.");
            }

            book.Title = dto.Title;
            book.ISBN = dto.ISBN;
            book.AuthorId = dto.AuthorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
