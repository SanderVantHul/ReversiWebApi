using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReversiWebApi.Models;
using System.Collections.Generic;
using System.Linq;
using ReversieISpelImplementatie.Model;
using System;

namespace ReversiWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpelController : ControllerBase
    {
        private readonly ISpelRepository iRepository;

        public SpelController(ISpelRepository repository)
        {
            iRepository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler()
        {
            return iRepository.GetSpellen().Where(s => s.Speler2Token == null).Select(s => s.Omschrijving).ToList();
        }

        [HttpPost]
        public ActionResult<string> PostAddSpel(string token, string omschrijving)
        {
            Spel spel = new Spel() { Speler1Token = token, Omschrijving = omschrijving };
            iRepository.AddSpel(spel);
            return Ok(spel.Token);
        }

        [HttpGet("{spelToken:guid}")]
        public ActionResult<Spel> GetSpel(string spelToken) 
        {
            Spel spel = iRepository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("{spelerToken}")]
        public ActionResult<Spel> GetSpelMetSpelerToken(string spelerToken)
        {
            Spel spel = iRepository.GetSpelMetSpelerToken(spelerToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("Beurt/{spelToken}")]
        public ActionResult<string> GetBeurt(string spelToken)
        {
            Spel spel = iRepository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel.AandeBeurt.ToString());
        }

        [HttpPut("Zet/{spelToken}")]
        public ActionResult<Spel> PutDoeZet(string spelToken)
        {
            Spel spel = iRepository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel.AandeBeurt.ToString());
        }

        [HttpPut("Opgeven")]
        public ActionResult<Spel> PutOpgeven(string spelToken)
        {
            Spel spel = iRepository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel.AandeBeurt.ToString());
        }
    }
}
