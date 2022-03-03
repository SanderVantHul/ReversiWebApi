using System.Collections.Generic;
using System.Threading.Tasks;
using ReversiWebApi.Models;

namespace ReversiWebApi.Repositories
{
    public interface ISpelRepository
    {
        Task AddSpel(Spel spel);

        Task<List<Spel>> GetSpellen();

        Task<Spel> GetSpel(string spelToken);

        Task<Spel> GetSpelMetSpelerToken(string spelerToken);

        Task Complete();
    }
}
