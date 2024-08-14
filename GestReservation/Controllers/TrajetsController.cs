using GestReservation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ReservationVoyage.Controllers
{
    public class TrajetsController : Controller
    {
        private readonly GestReservationContext _context;

        public TrajetsController(GestReservationContext context)
        {
            _context = context;
        }

        // GET: Trajets
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 5)
        {
            ViewData["CurrentFilter"] = searchString;

            var trajets = from t in _context.Trajets.Include(t => t.Utilisateur)
                          select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                trajets = trajets.Where(t => t.VilleDepart.Contains(searchString)
                                           || t.VilleArrive.Contains(searchString)
                                           || t.DateDepart.ToString().Contains(searchString)
                                           || t.Utilisateur.Prenom.Contains(searchString)
                                           || t.Utilisateur.Nom.Contains(searchString));
            }

            trajets = trajets.OrderBy(t => t.TrajetId);

            var paginatedTrajets = await PaginatedList<Trajet>.CreateAsync(trajets, pageNumber, pageSize);
            return View(paginatedTrajets);
        }

        // GET: Trajets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trajet = await _context.Trajets
                .Include(t => t.Utilisateur)
                .FirstOrDefaultAsync(m => m.TrajetId == id);
            if (trajet == null)
            {
                return NotFound();
            }

            return View(trajet);
        }

        // GET: Trajets/Create
        public IActionResult Create()
        {
            List<string> villes = new List<string> { "Touba", "Thies", "Dakar", "Guédiawaye", "Pikine", "Rufisque", "Kaolack", "Ziguinchor", "Louga", "Kolda", "Sédhiou", "Kayar" };
            ViewBag.Villes = new SelectList(villes);

            var utilisateurs = _context.Utilisateurs
                                       .Select(u => new { u.UtilisateurId, NomComplet = u.Prenom + " " + u.Nom })
                                       .ToList();
            ViewBag.Utilisateurs = new SelectList(utilisateurs, "UtilisateurId", "NomComplet");
            return View();
        }

        // POST: Trajets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrajetId,VilleDepart,VilleArrive,DateDepart,UtilisateurId")] Trajet trajet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trajet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            List<string> villes = new List<string> { "Touba", "Thies", "Dakar", "Guédiawaye", "Pikine", "Rufisque", "Kaolack", "Ziguinchor", "Louga", "Kolda", "Sédhiou", "Kayar" };
            ViewBag.Villes = new SelectList(villes);
            var utilisateurs = _context.Utilisateurs
                                                   .Select(u => new { u.UtilisateurId, NomComplet = u.Prenom + " " + u.Nom })
                                                   .ToList();
            ViewBag.Utilisateurs = new SelectList(utilisateurs, "UtilisateurId", "NomComplet", trajet.UtilisateurId);
            return View(trajet);
        }

        // GET: Trajets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trajet = await _context.Trajets.FindAsync(id);
            if (trajet == null)
            {
                return NotFound();
            }
            List<string> villes = new List<string> { "Touba", "Thies", "Dakar", "Guédiawaye", "Pikine", "Rufisque", "Kaolack", "Ziguinchor", "Louga", "Kolda", "Sédhiou", "Kayar" };
            ViewBag.Villes = new SelectList(villes);

            var utilisateurs = _context.Utilisateurs
                                                   .Select(u => new { u.UtilisateurId, NomComplet = u.Prenom + " " + u.Nom })
                                                   .ToList();
            ViewBag.Utilisateurs = new SelectList(utilisateurs, "UtilisateurId", "NomComplet");
            return View(trajet);
        }

        // POST: Trajets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrajetId,VilleDepart,VilleArrive,DateDepart,UtilisateurId")] Trajet trajet)
        {
            if (id != trajet.TrajetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trajet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrajetExists(trajet.TrajetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            List<string> villes = new List<string> { "Touba", "Thies", "Dakar", "Guédiawaye", "Pikine", "Rufisque", "Kaolack", "Ziguinchor", "Louga", "Kolda", "Sédhiou", "Kayar" };
            ViewBag.Villes = new SelectList(villes);

            var utilisateurs = _context.Utilisateurs
                                                   .Select(u => new { u.UtilisateurId, NomComplet = u.Prenom + " " + u.Nom })
                                                   .ToList();
            ViewBag.Utilisateurs = new SelectList(utilisateurs, "UtilisateurId", "NomComplet", trajet.UtilisateurId);
            return View(trajet);
        }

        // GET: Trajets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trajet = await _context.Trajets
                .Include(t => t.Utilisateur)
                .FirstOrDefaultAsync(m => m.TrajetId == id);
            if (trajet == null)
            {
                return NotFound();
            }

            return View(trajet);
        }

        // POST: Trajets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trajet = await _context.Trajets.FindAsync(id);
            if (trajet != null)
            {
                _context.Trajets.Remove(trajet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrajetExists(int id)
        {
            return _context.Trajets.Any(e => e.TrajetId == id);
        }
    }
}
