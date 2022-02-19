using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReversiWebApi.Models;
using System.Collections.Generic;
using System.Linq;
using ReversieISpelImplementatie.Model;
using System;

namespace ReversiWebApi.Controllers
{
    [Route("api/Spel")]
    [ApiController]
    public class SpelController : ControllerBase
    {
        private readonly ISpelRepository iRepository;

        public SpelController(ISpelRepository repository)
        {
            iRepository = repository;
        }

        // GET api/spel
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler()
        {
            return iRepository.GetSpellen().Where(s => s.Speler2Token == null).Select(s => s.Omschrijving).ToList();
        }

        [HttpPost]
        public ActionResult<string> AddSpel(string token, string omschrijving)
        {
            Spel spel = new Spel() { Token = Guid.NewGuid().ToString(), Speler1Token = token, Omschrijving = omschrijving };
            iRepository.AddSpel(spel);
            return Ok(spel.Token);
        }

        [HttpGet("{spelToken}")]
        public ActionResult<Spel> GetSpel(string spelToken)
        {
            Spel spel = iRepository.GetSpel(spelToken);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

    }
}
