
using GestReservation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ReservationVoyage.Controllers
{
    public class VehiculesController : Controller
    {
        private readonly GestReservationContext _context;

        public VehiculesController(GestReservationContext context)
        {
            _context = context;
        }
        public IActionResult GetImage(int id)
        {
            var vehicule = _context.Vehicules.Find(id);
            if (vehicule == null || vehicule.Image == null)
            {
                return NotFound();
            }
            return File(vehicule.Image, "image/jpeg");
        }


        // GET: Vehicules
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 5)
        {
            ViewData["CurrentFilter"] = searchString;

            var vehicules = from v in _context.Vehicules.Include(v => v.Utilisateur)
                            select v;

            if (!String.IsNullOrEmpty(searchString))
            {
                vehicules = vehicules.Where(v => v.Marque.Contains(searchString)
                                               || v.Modele.Contains(searchString)
                                               || v.Immatriculation.Contains(searchString)
                                               || v.Utilisateur.Prenom.Contains(searchString)
                                               || v.Utilisateur.Nom.Contains(searchString));
            }

            vehicules = vehicules.OrderBy(v => v.VehiculeId);

            var paginatedVehicules = await PaginatedList<Vehicule>.CreateAsync(vehicules, pageNumber, pageSize);
            return View(paginatedVehicules);
        }

        // GET: Vehicules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicule = await _context.Vehicules
                .Include(v => v.Utilisateur)
                .FirstOrDefaultAsync(m => m.VehiculeId == id);
            if (vehicule == null)
            {
                return NotFound();
            }

            return View(vehicule);
        }

        // GET: Vehicules/Create
        public IActionResult Create()
        {
            List<string> marques = new List<string> { "BMW", "Renault", "Mercedes", "Peugeot", "Toyota", "Audi", "Honda", "Ford", "Saab" };
            ViewBag.Marque = new SelectList(marques);

            var utilisateurs = _context.Utilisateurs
                                .Select(u => new { u.UtilisateurId, NomComplet = u.Prenom + " " + u.Nom })
                                .ToList();
            ViewBag.Utilisateurs = new SelectList(utilisateurs, "UtilisateurId", "NomComplet");
            return View();
        }

        // POST: Vehicules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("VehiculeId,Marque,Modele,Immatriculation,UtilisateurId")] Vehicule vehicule, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    vehicule.Image = "/assets/" + fileName;
                }

                _context.Add(vehicule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            List<string> marques = new List<string> { "BMW", "Renault", "Mercedes", "Peugeot", "Toyota", "Audi", "Honda", "Ford", "Saab" };
            ViewBag.Marque = new SelectList(marques);

            var utilisateurs = _context.Utilisateurs
                                .Select(u => new { u.UtilisateurId, NomComplet = u.Prenom + " " + u.Nom })
                                .ToList();
            ViewBag.Utilisateurs = new SelectList(utilisateurs, "UtilisateurId", "NomComplet", vehicule.UtilisateurId);
            return View(vehicule);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicule = await _context.Vehicules.FindAsync(id);
            if (vehicule == null)
            {
                return NotFound();
            }

            List<string> marques = new List<string> { "BMW", "Renault", "Mercedes", "Peugeot", "Toyota", "Audi", "Honda", "Ford", "Saab" };
            ViewBag.Marque = new SelectList(marques);

            var utilisateurs = _context.Utilisateurs
                                       .Select(u => new { u.UtilisateurId, NomComplet = u.Prenom + " " + u.Nom })
                                       .ToList();
            ViewBag.Utilisateurs = new SelectList(utilisateurs, "UtilisateurId", "NomComplet", vehicule.UtilisateurId);
            return View(vehicule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehiculeId,Marque,Modele,Immatriculation,UtilisateurId,Image")] Vehicule vehicule, IFormFile imageFile)
        {
            if (id != vehicule.VehiculeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        vehicule.Image = "/assets/" + fileName;
                    }

                    _context.Update(vehicule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculeExists(vehicule.VehiculeId))
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

            List<string> marques = new List<string> { "BMW", "Renault", "Mercedes", "Peugeot", "Toyota", "Audi", "Honda", "Ford", "Saab" };
            ViewBag.Marque = new SelectList(marques);

            var utilisateurs = _context.Utilisateurs
                                       .Select(u => new { u.UtilisateurId, NomComplet = u.Prenom + " " + u.Nom })
                                       .ToList();
            ViewBag.Utilisateurs = new SelectList(utilisateurs, "UtilisateurId", "NomComplet", vehicule.UtilisateurId);
            return View(vehicule);
        }

        // GET: Vehicules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicule = await _context.Vehicules
                .Include(v => v.Utilisateur)
                .FirstOrDefaultAsync(m => m.VehiculeId == id);
            if (vehicule == null)
            {
                return NotFound();
            }

            return View(vehicule);
        }

        // POST: Vehicules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicule = await _context.Vehicules.FindAsync(id);
            if (vehicule != null)
            {
                _context.Vehicules.Remove(vehicule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehiculeExists(int id)
        {
            return _context.Vehicules.Any(e => e.VehiculeId == id);
        }
    }
}
