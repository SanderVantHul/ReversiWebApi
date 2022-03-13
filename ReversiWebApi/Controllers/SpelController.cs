using System;
using Microsoft.AspNetCore.Mvc;
using ReversiWebApi.Models;
using ReversiWebApi.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Spel>>> GetSpellenMetWachtendeSpeler()
        {
            return Ok(await _repository.GetSpellenMetWachtendeSpeler());
        }

        [HttpGet("{spelToken}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Spel>> GetSpel(string spelToken)
        {
            Spel spel = await _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("~/api/Speler/{spelerToken}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Spel>> GetSpelMetSpelerToken(string spelerToken)
        {
            Spel spel = await _repository.GetSpelMetSpelerToken(spelerToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("Beurt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetBeurt(string spelToken)
        {
            Spel spel = await _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel.AandeBeurt.ToString());
        }

        //[HttpPost("{omschrijving}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<Spel>> ToevoegenSpel([FromBody] string token, string omschrijving)
        //{
        //    Spel spel = new Spel() { Speler1Token = token, Omschrijving = omschrijving };
        //    await _repository.AddSpel(spel);
        //    return Ok(spel.Token);
        //}

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Spel>> ToevoegenSpel(Spel spel)
        {
            var nieuwSpel = new Spel() { Speler1Token = spel.Speler1Token, Omschrijving = spel.Omschrijving};
            await _repository.AddSpel(nieuwSpel);
            return Ok(nieuwSpel.Token);
        }

        [HttpPut("Zet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
