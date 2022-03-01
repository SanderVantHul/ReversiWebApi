using Microsoft.AspNetCore.Mvc;
using ReversiWebApi.Models;
using ReversiWebApi.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace ReversiWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpelController : ControllerBase
    {
        private readonly ISpelRepository _repository;

        public SpelController(ISpelRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler()
        {
            return _repository.GetSpellen().Where(s => s.Speler2Token == null).Select(s => s.Omschrijving).ToList();
        }

        [HttpPost]
        public ActionResult<Spel> PostToevoegenSpel([FromBody]string token, string omschrijving)
        {
            Spel spel = new Spel() { Speler1Token = token, Omschrijving = omschrijving };
            _repository.AddSpel(spel);
            return Ok(spel.Token);
        }

        [HttpPost("Spel2")]
        public ActionResult<Spel> PostToevoegenSpel([FromBody]Spel spel)
        {
            _repository.AddSpel(spel);
            return Ok(spel.Token);
        }

        [HttpGet("{spelToken}")]
        public ActionResult<Spel> GetSpel(string spelToken) 
        {
            Spel spel = _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("Speler/{spelerToken}")]
        public ActionResult<Spel> GetSpelMetSpelerToken(string spelerToken)
        {
            Spel spel = _repository.GetSpelMetSpelerToken(spelerToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("Beurt/{spelToken}")]
        public ActionResult<string> GetBeurt(string spelToken)
        {
            Spel spel = _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel.AandeBeurt.ToString());
        }

        [HttpPut("Zet/{spelToken}")]
        public ActionResult<string> PutDoeZet(string spelToken)
        {
            Spel spel = _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel.AandeBeurt.ToString());
        }

        [HttpPut("Opgeven")]
        public ActionResult<string> PutOpgeven(string spelToken)
        {
            Spel spel = _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel.AandeBeurt.ToString());
        }
    }
}
