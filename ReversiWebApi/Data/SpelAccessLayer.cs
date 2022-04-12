using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReversiWebApi.Models;
using ReversiWebApi.Repositories;

namespace ReversiWebApi.Data
{
    public class SpelAccessLayer : ISpelRepository
    {
        private ReversiContext _context;
        private SpelerContext _spelerContext;

        public SpelAccessLayer(ReversiContext context, SpelerContext spelerContext) // is injected in Startup.cs
        {
            _context = context;
            _spelerContext = spelerContext;
        }

        public async Task<Spel> GetSpel(string spelToken) => await _context.Spellen.FirstOrDefaultAsync(s => s.Token == spelToken);

        public async Task<List<Spel>> GetSpellen() => await _context.Spellen.ToListAsync();

        public async Task<List<Spel>> GetSpellenMetWachtendeSpeler() => await _context.Spellen.Where(s => s.Speler2Token == null).ToListAsync();

        public async Task<Spel> GetSpelMetSpelerToken(string spelerToken) =>
            await _context.Spellen.FirstOrDefaultAsync(s => s.Speler1Token == spelerToken || s.Speler2Token == spelerToken);

        public async Task AddSpel(Spel spel)
        {
            await _context.Spellen.AddAsync(spel);
            await _context.SaveChangesAsync();
        }

        public async Task JoinSpel(SpelViewModel spelModel)
        {
            var spel = await _context.Spellen.FirstOrDefaultAsync(s => s.Token == spelModel.SpelToken);
            if (spel == null) return;
            spel.Speler2Token = spelModel.SpelerToken;
            spel.AandeBeurt = Kleur.Zwart;
            await _context.SaveChangesAsync();
        }
            
        public async Task Opgeven(Spel spel, string idSpelerDieOpgaf)
        {
            var verlorenSpeler = await _spelerContext.Spelers.FirstOrDefaultAsync(s => s.Guid == idSpelerDieOpgaf);
            if(verlorenSpeler != null) verlorenSpeler.AantalVerloren++;
            
            var idSpelerDieWon = spel.Speler1Token == idSpelerDieOpgaf ? spel.Speler2Token : spel.Speler1Token;
            var gewonnenSpeler = await _spelerContext.Spelers.FirstOrDefaultAsync(s => s.Guid == idSpelerDieWon);
            if(gewonnenSpeler != null) gewonnenSpeler.AantalGewonnen++;
            _context.Spellen.Remove(spel);
            await _spelerContext.SaveChangesAsync();
            await _context.SaveChangesAsync();
        }

        public async Task SpelVerwijderen(Spel spel)
        {
            if (spel.AantalWit > spel.AantalZwart)
            {
                var gewonnenSpeler= await _spelerContext.Spelers.FirstOrDefaultAsync(s => s.Guid == spel.Speler1Token);
                if(gewonnenSpeler != null) gewonnenSpeler.AantalGewonnen++;

                var verlorenSpeler = await _spelerContext.Spelers.FirstOrDefaultAsync(s => s.Guid == spel.Speler2Token);
                if(verlorenSpeler != null) verlorenSpeler.AantalVerloren++;
            }
            else if(spel.AantalWit < spel.AantalZwart)
            {
                var verlorenSpeler = await _spelerContext.Spelers.FirstOrDefaultAsync(s => s.Guid == spel.Speler1Token);
                if(verlorenSpeler != null) verlorenSpeler.AantalVerloren++;

                var gewonnenSpeler= await _spelerContext.Spelers.FirstOrDefaultAsync(s => s.Guid == spel.Speler2Token);
                if(gewonnenSpeler != null) gewonnenSpeler.AantalGewonnen++;
            }
            else
            {
                var verlorenSpeler = await _spelerContext.Spelers.FirstOrDefaultAsync(s => s.Guid == spel.Speler1Token);
                if(verlorenSpeler != null) verlorenSpeler.AantalGelijk++;

                var gewonnenSpeler= await _spelerContext.Spelers.FirstOrDefaultAsync(s => s.Guid == spel.Speler2Token);
                if(gewonnenSpeler != null) gewonnenSpeler.AantalGelijk++;
            }

            _context.Spellen.Remove(spel);
            await _spelerContext.SaveChangesAsync();
            await _context.SaveChangesAsync();
        }

        public async Task Complete() => await _context.SaveChangesAsync();
    }
}
