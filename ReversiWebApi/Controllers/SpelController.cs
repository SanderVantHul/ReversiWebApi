using System;
using Microsoft.AspNetCore.Mvc;
using ReversiWebApi.Models;
using ReversiWebApi.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<string>>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler()
        {
            var spellen = await _repository.GetSpellen();
            return spellen.Where(s => s.Speler2Token == null).Select(s => s.Omschrijving).ToList();
        }

        [HttpGet("{spelToken}")]
        public async Task<ActionResult<Spel>> GetSpel([FromBody] string spelToken)
        {
            Spel spel = await _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("Speler")]
        public async Task<ActionResult<Spel>> GetSpelMetSpelerToken([FromBody] string spelerToken)
        {
            Spel spel = await _repository.GetSpelMetSpelerToken(spelerToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("Beurt")]
        public async Task<ActionResult<string>> GetBeurt([FromBody] string spelToken)
        {
            Spel spel = await _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel.AandeBeurt.ToString());
        }

        [HttpPost]
        public async Task<ActionResult<Spel>> ToevoegenSpel([FromBody] string token, string omschrijving)
        {
            Spel spel = new Spel() { Speler1Token = token, Omschrijving = omschrijving };
            await _repository.AddSpel(spel);
            return Ok(spel.Token);
        }

        [HttpPost("AddSpel/TestJsonConverter")]
        public async Task<ActionResult<Spel>> ToevoegenSpel(Spel spel)
        {
            await _repository.AddSpel(spel);
            return Ok(spel.Token);
        }

        [HttpPut("Zet")]
        public async Task<ActionResult<Spel>> DoeZet([FromForm] string spelToken, [FromForm] string spelerToken, int rij, int kolom)
        {
            Spel spelResult = await _repository.GetSpel(spelToken);
            // validation
            if (spelResult == null) return NotFound();
            if (spelResult.Speler1Token != spelerToken && spelResult.Speler2Token != spelerToken) return Unauthorized("speler niet in dit spel");
            if ((spelResult.Speler1Token == spelerToken ? Kleur.Wit : Kleur.Zwart) != spelResult.AandeBeurt) return Unauthorized("speler niet aan de beurt");

            try
            {
                spelResult.DoeZet(rij, kolom);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            await _repository.Complete();

            return Ok(spelResult);
        }

        [HttpPut("Passen")]
        public async Task<ActionResult<string>> Passen([FromForm] string spelToken, [FromForm] string spelerToken)
        {
            Spel spel = await _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            if (spel.Speler1Token != spelerToken && spel.Speler2Token != spelerToken) return Unauthorized("speler niet in dit spel");
            if ((spel.Speler1Token == spelerToken ? Kleur.Wit : Kleur.Zwart) != spel.AandeBeurt) return Unauthorized("speler niet aan de beurt");

            try
            {
                spel.Pas();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            await _repository.Complete();

            return Ok("Speler heeft beurt gepassed");
        }
    }
}
