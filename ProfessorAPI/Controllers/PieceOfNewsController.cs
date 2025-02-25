using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;
using System.Text.RegularExpressions;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PieceOfNewsController : Controller
    {
        private readonly DimajProfessorsDbContext _context;

        public PieceOfNewsController(DimajProfessorsDbContext context)
        {
            _context = context;
        }
        // GET: api/PieceOfNews/GetPieceOfNews
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<PieceOfNews>>> GetPieceOfNews()
        {
            var newsList = await _context.PieceOfNews
                
                .Include(n => n.Author) // Incluye los datos del autor (User)
                .OrderByDescending(n => n.Date) // Ordena por fecha descendente
                .Select(n => new
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Description,
                    Picture = n.Picture != null ? "data:image/jpeg;base64," + Convert.ToBase64String(n.Picture) : null,
                    Date = n.Date,
                    User = n.Author != null ? new User
                    {
                        Id = n.Author.Id,
                        Name = n.Author.Name,
                        Role = n.Author.Role,
                        IsActive = n.Author.IsActive ?? false, // Si es null, asigna `false`
                    } : null
                })
                .ToListAsync();

            return Ok(newsList);
        }


        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult<PieceOfNews>> GetPieceOfNews(string id)
        {
            var news = await _context.PieceOfNews
                .Include(n => n.Author) // Incluye el autor
                .Where(n => n.Id == id)
                .Select(n => new 
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Description,
                    Picture = n.Picture != null ? "data:image/jpeg;base64," + Convert.ToBase64String(n.Picture) : null,
                    Date = n.Date,
                    User = n.Author != null ? new User
                    {
                        Id = n.Author.Id,
                        Name = n.Author.Name,
                        Role = n.Author.Role,
                        IsActive = n.Author.IsActive ?? false, // Si es null, asigna `false`
                    } : null
                })
                .FirstOrDefaultAsync();


            return Ok(news);
        }



        //-------------MÉTODOS DEL MVC A LA API:----------------\\

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> AddPieceOfNews([FromBody] CreatePieceOfNewsDTO newsDTO)
        {
            try
            {
                string base64String = newsDTO.Picture;
                base64String = Regex.Replace(base64String, "^data:.+;base64,", "");

                // Convertir la cadena base64 a un arreglo de bytes
                byte[] imageBytes = Convert.FromBase64String(base64String);

                var newPieceOfNews = new PieceOfNews
                {
                    Id = newsDTO.Id,
                    Title = newsDTO.Title,
                    Description = newsDTO.Description,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                    Picture = imageBytes,
                    AuthorId = newsDTO.UserId
                };

                _context.PieceOfNews.Add(newPieceOfNews);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Piece of news added successfully..." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred adding the Piece of news...", Error = ex.Message });
            }
        }
        [HttpPost]
        [Route("[action]/{newsId}")]
        public async Task<ActionResult> DeletePieceOfNews(string newsId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Eliminar respuestas de comentarios asociadas a la noticia
                var commentResponses = _context.CommentNewsResponses
                    .Where(cr => _context.CommentNews
                        .Where(c => c.PieceOfNewsId == newsId)
                        .Select(c => c.Id)
                        .Contains(cr.CommentNewsId));

                _context.CommentNewsResponses.RemoveRange(commentResponses);
                await _context.SaveChangesAsync();

                // Eliminar comentarios asociados a la noticia
                var comments = _context.CommentNews.Where(c => c.PieceOfNewsId == newsId);
                _context.CommentNews.RemoveRange(comments);
                await _context.SaveChangesAsync();

                // Eliminar la noticia
                var pieceOfNews = await _context.PieceOfNews.FindAsync(newsId);
                if (pieceOfNews != null)
                {
                    _context.PieceOfNews.Remove(pieceOfNews);
                    await _context.SaveChangesAsync();

                }


                // Confirmar la transacción
                await transaction.CommitAsync();

                return Ok(new { Message = "Piece of news deleted successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "An error occurred deleting the Piece of news...", Error = ex.Message });
            }
        }


    }
}
