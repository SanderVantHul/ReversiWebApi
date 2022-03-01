using Microsoft.EntityFrameworkCore;
using ReversiWebApi.Models;

namespace ReversiWebApi.Data
{
    public class ReversiContext : DbContext
    {
        public ReversiContext(DbContextOptions<ReversiContext> options) : base(options) { }

        public DbSet<Spel> Spellen { get; set; }
    }
}