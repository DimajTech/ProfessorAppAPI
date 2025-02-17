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
                    Picture = imageBytes,
                    Author = new User
                    {
                        Id = newsDTO.UserId,
                        Role = newsDTO.UserRole
                    }
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
        /*
        //NECESARIO
        [HttpPost]
        [Route("InsertNews")]
        public async Task<IActionResult> InsertNews([FromBody] InsertNewsDTO newsDTO)
        {
            try
            {
                // Convertir Base64 a byte[]
                byte[] pictureBytes = Convert.FromBase64String(newsDTO.Picture);

                var newPieceOfNews = new PieceOfNews
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = newsDTO.Title,
                    Picture = pictureBytes, // Guardamos la imagen convertida
                    AuthorId = newsDTO.AuthorId,
                    Description = newsDTO.Description,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    File = null
                };

                _context.PieceOfNews.Add(newPieceOfNews);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "News inserted successfully", NewsId = newPieceOfNews.Id });
            }
            catch (FormatException)
            {
                return BadRequest(new { Message = "Invalid Base64 format" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error inserting news", Error = ex.Message });
            }
        }
        */

    }
}
