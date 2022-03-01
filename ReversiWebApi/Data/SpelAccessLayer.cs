using System.Collections.Generic;
using System.Linq;
using ReversiWebApi.Models;
using ReversiWebApi.Repositories;

namespace ReversiWebApi.Data
{
    public class SpelAccessLayer : ISpelRepository
    {
        private ReversiContext _context;

        public SpelAccessLayer(ReversiContext context) // is injected in startup class
        {
            _context = context;
        }

        public void AddSpel(Spel spel)
        {
            _context.Spellen.Add(spel);
            _context.SaveChanges();
        }

        public Spel GetSpel(string spelToken) => _context.Spellen.First(s => s.Token == spelToken);

        public List<Spel> GetSpellen() => _context.Spellen.ToList();

        public Spel GetSpelMetSpelerToken(string spelerToken) =>
            _context.Spellen.First(s => s.Speler1Token == spelerToken || s.Speler2Token == spelerToken);
    }
}
