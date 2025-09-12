using GestionTaches.Models;
using GestionTaches.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestionTaches.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GestionTachesController : ControllerBase
    {
        private readonly ILogger<GestionTachesController> _logger;
        private readonly TacheRepository repo;

        public GestionTachesController(ILogger<GestionTachesController> logger)
        {
            _logger = logger;
            repo = new TacheRepository(); // on peut plus tard injecter via DI
        }

        // 1. Ajouter une t�che
        [HttpPost("ajouter")]
        public IActionResult AjouterTache([FromBody] Taches tache)
        {
            if (tache == null || string.IsNullOrEmpty(tache.Nom))
                return BadRequest("T�che invalide.");

            tache.DateCreation = DateTime.Now;
            repo.AddTache(tache);
            return Ok(tache);
        }

        // 2. Lister les t�ches
        [HttpGet("liste")]
        public IEnumerable<Taches> GetListeTaches()
        {
            return repo.GetTaches();
        }

        // 3. Marquer une t�che comme termin�e
        [HttpPut("terminer/{id}")]
        public IActionResult MarquerTerminee(int id)
        {
            var tache = repo.GetTaches().FirstOrDefault(t => t.Id == id);
            if (tache == null)
                return NotFound($"T�che {id} non trouv�e.");

            repo.MarkCompleted(id);
            return Ok(tache);
        }

        // 4. Supprimer une t�che
        [HttpDelete("supprimer/{id}")]
        public IActionResult SupprimerTache(int id)
        {
            var tache = repo.GetTaches().FirstOrDefault(t => t.Id == id);
            if (tache == null)
                return NotFound($"T�che {id} non trouv�e.");

            repo.DeleteTache(id);
            return Ok($"T�che {id} supprim�e.");
        }
    }
}
