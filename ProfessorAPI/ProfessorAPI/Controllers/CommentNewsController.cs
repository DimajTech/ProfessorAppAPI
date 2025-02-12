using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentNewsController : Controller
    {
        private readonly DimajProfessorsDbContext _context;

        public CommentNewsController(DimajProfessorsDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("[action]/{pieceOfNewsId}")]
        public async Task<ActionResult<IEnumerable<CommentNews>>> GetCommentsByPieceOfNewsId(string pieceOfNewsId)
        {
            var comments = await _context.CommentNews
                .Where(c => c.PieceOfNewsId == pieceOfNewsId)
                .Include(c => c.Author) //Incluir la información del autor
                .Select(c => new
                {
                    Id = c.Id,
                    Text = c.Text,
                    DateTime = c.Date ?? DateTime.UtcNow,
                    User = new User
                    {
                        Id = c.Author.Id,
                        Name = c.Author.Name ?? "",
                        Role = c.Author.Role ?? "",
                        IsActive = c.Author.IsActive ?? false, //Si es nulo, asigna false

                    },
                    TotalResponses = _context.CommentNewsResponses.Count(r => r.CommentNewsId == c.Id)
                })
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound();
            }

            return Ok(comments);
        }

        [HttpGet]
        [Route("[action]/{commentNewsId}")]
        public async Task<ActionResult<IEnumerable<CommentNewsResponse>>> GetResponsesByCommentNewsId(string commentNewsId)
        {
            var responses = await _context.CommentNewsResponses
                .Where(c => c.CommentNewsId == commentNewsId)
                .Include(c => c.Author) // Incluir información del autor
                .Select(c => new
                {
                    Id = c.Id,
                    Text = c.Text,
                    DateTime = c.Date ?? DateTime.UtcNow,
                    User = new User
                    {
                        Id = c.Author.Id,
                        Name = c.Author.Name ?? "",
                        Role = c.Author.Role ?? "",
                        IsActive = c.Author.IsActive ?? false, //Si es nulo, asigna false

                    }
                })
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();

            if (responses == null || !responses.Any())
            {
                return NotFound();
            }

            return Ok(responses);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> AddNewsComment([FromBody] CommentNewsDTO commentDto)
        {
            try
            {
                var newComment = new CommentNews
                {
                    Id = Guid.NewGuid().ToString(),
                    PieceOfNewsId = commentDto.PieceOfNewsId,
                    AuthorId = commentDto.AuthorId,
                    Text = commentDto.Text,
                    Date = DateTime.UtcNow
                };

                _context.CommentNews.Add(newComment);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Comment added successfully...", CommentId = newComment.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred adding...", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> AddNewsCommentResponse([FromBody] CommentNewsResponseDTO commentResponseDto)
        {
            try
            {
                var newResponse = new CommentNewsResponse
                {
                    Id = Guid.NewGuid().ToString(),
                    CommentNewsId = commentResponseDto.CommentNewsId,
                    AuthorId = commentResponseDto.AuthorId,
                    Text = commentResponseDto.Text,
                    Date = DateTime.UtcNow
                };

                _context.CommentNewsResponses.Add(newResponse);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Response added successfully..." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred adding...", Error = ex.Message });
            }
        }


    }
}
