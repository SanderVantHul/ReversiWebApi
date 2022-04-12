using Microsoft.EntityFrameworkCore;
using ReversiWebApi.Models;

namespace ReversiWebApi.Data
{
    public class SpelerContext : DbContext
    {
        public SpelerContext(DbContextOptions<SpelerContext> options) : base(options) { }
        public DbSet<Speler> Spelers { get; set; }
    }
}