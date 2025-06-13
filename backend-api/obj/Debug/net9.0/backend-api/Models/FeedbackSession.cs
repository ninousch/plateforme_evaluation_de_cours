using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CoursFeedback.API.Models
{
    public class FeedbackSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using CoursFeedback.API.Models;   // Remplacez par le namespace de vos modèles
public DbSet<FeedbackSession> FeedbackSessions { get; set; }



namespace CoursFeedback.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<FeedbackSession> FeedbackSessions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Contrainte unique : une seule session active par cours
            builder.Entity<FeedbackSession>()
                   .HasIndex(f => new { f.CourseId, f.IsActive })
                   .HasFilter("[IsActive] = 1")
                   .IsUnique();
        }
        
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursFeedback.API.Data;
using CoursFeedback.API.Models;

namespace CoursFeedback.API.Models
{
    [ApiController]
    [Route("api/feedback/sessions")]
    [Authorize(Roles = "Admin")]
    public class FeedbackSessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public FeedbackSessionsController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.FeedbackSessions
                                .Include(f => f.Course)
                                .OrderByDescending(f => f.StartDate)
                                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var session = await _db.FeedbackSessions
                                   .Include(f => f.Course)
                                   .FirstOrDefaultAsync(f => f.Id == id);
            if (session == null) return NotFound();
            return Ok(session);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackSession model)
        {
            if (model.StartDate > model.EndDate)
                return BadRequest("La date de début doit être antérieure à la date de fin.");

            if (model.IsActive &&
                await _db.FeedbackSessions.AnyAsync(f => f.CourseId == model.CourseId && f.IsActive))
            {
                return BadRequest("Une session active existe déjà pour ce cours.");
            }

            _db.FeedbackSessions.Add(model);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOne), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] FeedbackSession model)
        {
            if (id != model.Id) return BadRequest("ID invalide.");
            if (model.StartDate > model.EndDate)
                return BadRequest("La date de début doit être antérieure à la date de fin.");

            if (model.IsActive &&
                await _db.FeedbackSessions.AnyAsync(f => f.CourseId == model.CourseId && f.IsActive && f.Id != id))
            {
                return BadRequest("Une session active existe déjà pour ce cours.");
            }

            _db.Entry(model).State = EntityState.Modified;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.FeedbackSessions.AnyAsync(f => f.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _db.FeedbackSessions.FindAsync(id);
            if (session == null) return NotFound();

            _db.FeedbackSessions.Remove(session);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using CoursFeedback.API.Data;

var builder = WebApplication.CreateBuilder(args);

// … le reste de votre configuration


// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddControllers();
builder.Services.AddAuthentication(/* JWT or Identity */);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("AdminOnly");

app.Run();

// Migration commands (CLI)
// dotnet ef migrations add InitFeedbackSessions
// dotnet ef database update
