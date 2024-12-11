using DemoWebApp.Data;
using DemoWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JokesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JokesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Отримання списку жартів з фільтруванням
        [HttpGet]
        public IActionResult GetJokes([FromQuery] string category, [FromQuery] int? rating, [FromQuery] DateTime? date)
        {
            var jokes = _context.Joke.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                jokes = jokes.Where(j => j.Category == category);
            }

            if (rating.HasValue)
            {
                jokes = jokes.Where(j => j.Rating >= rating.Value);
            }

            if (date.HasValue)
            {
                jokes = jokes.Where(j => j.CreatedDate.Date == date.Value.Date);
            }

            return Ok(jokes.ToList());
        }

        // 2. Отримання конкретного жарту за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJoke(int id)
        {
            var joke = await _context.Joke.FindAsync(id);
            if (joke == null)
            {
                return NotFound();
            }
            return Ok(joke);
        }

        // 3. Додавання нового жарту
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddJoke([FromBody] Joke joke)
        {
            if (ModelState.IsValid)
            {
                _context.Joke.Add(joke);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetJoke), new { id = joke.Id }, joke);
            }

            return BadRequest(ModelState);
        }

        // 4. Оновлення жарту
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJoke(int id, [FromBody] Joke joke)
        {
            if (id != joke.Id)
            {
                return BadRequest();
            }

            var existingJoke = await _context.Joke.FindAsync(id);
            if (existingJoke == null)
            {
                return NotFound();
            }

            existingJoke.JokeQuestion = joke.JokeQuestion;
            existingJoke.JokeAnswer = joke.JokeAnswer;
            existingJoke.Category = joke.Category;
            existingJoke.Rating = joke.Rating;
            existingJoke.CreatedDate = joke.CreatedDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. Видалення жарту
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJoke(int id)
        {
            var joke = await _context.Joke.FindAsync(id);
            if (joke == null)
            {
                return NotFound();
            }

            _context.Joke.Remove(joke);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
