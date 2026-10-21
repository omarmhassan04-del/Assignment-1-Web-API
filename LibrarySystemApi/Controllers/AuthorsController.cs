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
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public AuthorsController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .AsNoTracking()
                .Select(a => new AuthorDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    BirthDate = a.BirthDate,
                    Books = a.Books.Select(b => new BookSummaryDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        ISBN = b.ISBN
                    }).ToList()
                })
                .ToListAsync();

            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetById(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AuthorDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    BirthDate = a.BirthDate,
                    Books = a.Books.Select(b => new BookSummaryDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        ISBN = b.ISBN
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var author = new Author
            {
                Name = dto.Name,
                BirthDate = dto.BirthDate
            };

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            var result = new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                BirthDate = author.BirthDate,
                Books = new List<BookSummaryDto>()
            };

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            author.Name = dto.Name;
            author.BirthDate = dto.BirthDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
