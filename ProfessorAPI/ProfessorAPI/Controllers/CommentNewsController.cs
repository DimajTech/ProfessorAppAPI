﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentNewsController : Controller
    {
        private readonly DimajProfessorsDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string STUDENT_APP_URL;

        public CommentNewsController(DimajProfessorsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            STUDENT_APP_URL = _configuration["EnvironmentVariables:STUDENT_APP_URL"];

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


        //TODO: LLamar al MVC en estos métodos

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

                //------------LLAMAR AL MVC-------------\\
                SendNewsComment(newComment);

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

                //------------LLAMAR AL MVC-------------\\
                SendNewsCommentResponse(newResponse);

                return Ok(new { Message = "Response added successfully..." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred adding...", Error = ex.Message });
            }
        }

        //-------------COMUNICARSE CON EL MVC:----------------\\

        private IActionResult SendNewsCommentResponse(CommentNewsResponse comment)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(STUDENT_APP_URL);

                    var responseFormat = new
                    {
                        Id = comment.Id,
                        CommentNewsId = comment.CommentNewsId,
                        AuthorId = comment.AuthorId.ToString(),
                        Text = comment.Text
                    };


                    var postTask = client.PostAsJsonAsync("/CommentNews/AddNewsCommentResponseFromAPI", responseFormat);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return Ok(new { Message = "Response added successfully" });
                    }
                    else
                    {
                        var errorMessage = result.Content.ReadAsStringAsync().Result;
                        return StatusCode((int)result.StatusCode, new { Message = "Failed to add Response", Error = errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Exception occurred", Error = ex.Message });
            }
        }

        private IActionResult SendNewsComment(CommentNews comment)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(STUDENT_APP_URL);

                    var commentFormat = new 
                    {
                        Id = comment.Id,
                        PieceOfNewsId = comment.PieceOfNewsId,
                        AuthorId = comment.AuthorId.ToString(),
                        Text = comment.Text
                    };


                    var postTask = client.PostAsJsonAsync("CommentNews/AddNewsCommentFromAPI", commentFormat);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return Ok(new { Message = "Comment added successfully" });
                    }
                    else
                    {
                        var errorMessage = result.Content.ReadAsStringAsync().Result;
                        return StatusCode((int)result.StatusCode, new { Message = "Failed to add comment", Error = errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Exception occurred", Error = ex.Message });
            }
        }

        //------------MVC con la API---------------\\

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> AddNewsCommentFromMVC([FromBody] CommentNewsDTO commentDto)
        {
            try
            {
                var newComment = new CommentNews
                {
                    Id = commentDto.Id,
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
        public async Task<ActionResult> AddNewsCommentResponseFromMVC([FromBody] CommentNewsResponseDTO commentResponseDto)
        {
            try
            {
                var newResponse = new CommentNewsResponse
                {
                    Id = commentResponseDto.Id,
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
