using System.Collections.Generic;
using ReversiWebApi.Models;

namespace ReversiWebApi.Repositories
{
    public interface ISpelRepository
    {
        void AddSpel(Spel spel);

        public List<Spel> GetSpellen();

        Spel GetSpel(string spelToken);

        Spel GetSpelMetSpelerToken(string spelerToken);
    }
}
