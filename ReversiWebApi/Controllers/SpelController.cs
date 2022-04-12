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

        [HttpGet("Afgelopen/{spelToken}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> SpelAfgelopen(string spelToken)
        {
            Spel spel = await _repository.GetSpel(spelToken);
            if (spel == null) return BadRequest();
            var afgelopen = spel.Afgelopen();
            return Ok(afgelopen);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Spel>> ToevoegenSpel(SpelViewModel spelModel)
        {
            var nieuwSpel = new Spel() { Speler1Token = spelModel.SpelerToken, Omschrijving = spelModel.Omschrijving};
            await _repository.AddSpel(nieuwSpel);
            return Ok(nieuwSpel.Token);
        }

        [HttpPost("~/api/Speler")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> JoinSpel(SpelViewModel spel)
        {
            await _repository.JoinSpel(spel);
            return Ok(spel.SpelToken);
        }

        [HttpPut("Zet/{rij}/{kolom}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Spel>> DoeZet(SpelViewModel spelModel, int rij, int kolom)
        {
            Spel spelResult = await _repository.GetSpel(spelModel.SpelToken);
            // validation
            if (spelResult == null) return NotFound();
            if (spelResult.Speler1Token != spelModel.SpelerToken && spelResult.Speler2Token != spelModel.SpelerToken) 
                return Unauthorized("speler niet in dit spel");
            if ((spelResult.Speler1Token == spelModel.SpelerToken ? Kleur.Wit : Kleur.Zwart) != spelResult.AandeBeurt) 
                return Unauthorized("speler niet aan de beurt");

            try
            {
                spelResult.DoeZet(kolom - 1, rij - 1);
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
        public async Task<ActionResult<Spel>> Passen(SpelViewModel spelModel)
        {
            Spel spel = await _repository.GetSpel(spelModel.SpelToken);
            if (spel == null) return NotFound();
            if (spel.Speler1Token != spelModel.SpelerToken && spel.Speler2Token != spelModel.SpelerToken) return Unauthorized("speler niet in dit spel");
            if ((spel.Speler1Token == spelModel.SpelerToken ? Kleur.Wit : Kleur.Zwart) != spel.AandeBeurt) return Unauthorized("speler niet aan de beurt");

            try
            {
                spel.Pas();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            spel.RemoveHighlights();
            spel.HighlightMogelijkeZetten(spel.AandeBeurt);

            await _repository.Complete();

            return Ok(spel);
        }

        [HttpDelete("~/api/Speler/{spelerId}")]
        public async Task<ActionResult<bool>> Opgeven(string spelerId)
        {
            Spel spel = await _repository.GetSpelMetSpelerToken(spelerId);
            if (spel == null) return NotFound();

            try
            {
                await _repository.Opgeven(spel, spelerId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpDelete("{spelToken}")]
        public async Task<ActionResult<bool>> SpelVerwijderen(string spelToken)
        {
            Spel spel = await _repository.GetSpel(spelToken);
            if (spel == null) return NotFound();

            try
            {
                await _repository.SpelVerwijderen(spel);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

    }
}
