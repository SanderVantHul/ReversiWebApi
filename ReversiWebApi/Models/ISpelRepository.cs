using ReversieISpelImplementatie.Model;
using System.Collections.Generic;

namespace ReversiWebApi.Models
{
    public interface ISpelRepository
    {
        void AddSpel(Spel spel);

        public List<Spel> GetSpellen();

        Spel GetSpel(string spelToken);

        Spel GetSpelMetSpelerToken(string spelerToken);
    }
}
