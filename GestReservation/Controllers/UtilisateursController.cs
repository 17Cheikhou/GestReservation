using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestReservation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestReservation.Models;

namespace GestReservation.Controllers
{
    public class UtilisateursController : Controller
    {
        private readonly GestReservationContext _context;

        public UtilisateursController(GestReservationContext context)
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

        // GET: Utilisateurs
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            var utilisateurs = _context.Utilisateurs.OrderBy(u => u.UtilisateurId);
            var paginatedUtilisateurs = await PaginatedList<Utilisateur>.CreateAsync(utilisateurs, pageNumber, pageSize);
            return View(paginatedUtilisateurs);
        }

        // GET: Utilisateurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(m => m.UtilisateurId == id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        // GET: Utilisateurs/Create
        public IActionResult Create()
        {
            List<string> roles = new List<string> { "Passager", "Conducteur" };
            ViewBag.Roles = new SelectList(roles);
            return View();
        }

        // POST: Utilisateurs/Create
        // POST: Utilisateurs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Prenom,Nom,Email,MotDePass,Roles")] Utilisateur utilisateur, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets", fileName);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        utilisateur.Image = "/assets/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        // Ajoutez des logs ou une gestion d'erreur ici
                        ModelState.AddModelError("", "Erreur lors du téléchargement de l'image : " + ex.Message);
                        List<string> role = new List<string> { "Passager", "Conducteur" };
                        ViewBag.Roles = new SelectList(role);
                        return View(utilisateur);
                    }
                }
                else
                {
                    utilisateur.Image = "/assets/default-image.jpg"; // Optionnel, image par défaut si aucune image n'est chargée
                }

                _context.Add(utilisateur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            List<string> roles = new List<string> { "Passager", "Conducteur" };
            ViewBag.Roles = new SelectList(roles);
            return View(utilisateur);
        }

        // GET: Utilisateurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            List<string> roles = new List<string> { "Passager", "Conducteur" };
            ViewBag.Roles = new SelectList(roles);
            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UtilisateurId,Prenom,Nom,Email,MotDePass,Roles")] Utilisateur utilisateur, IFormFile imageFile)
        {
            if (id != utilisateur.UtilisateurId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var utilisateurToUpdate = await _context.Utilisateurs.FindAsync(id);

                    if (utilisateurToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Mettez à jour les propriétés modifiées
                    utilisateurToUpdate.Prenom = utilisateur.Prenom;
                    utilisateurToUpdate.Nom = utilisateur.Nom;
                    utilisateurToUpdate.Email = utilisateur.Email;
                    utilisateurToUpdate.MotDePass = utilisateur.MotDePass;
                    utilisateurToUpdate.Roles = utilisateur.Roles;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        utilisateurToUpdate.Image = "/assets/" + fileName;
                    }

                    _context.Update(utilisateurToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilisateurExists(id))
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
            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(m => m.UtilisateurId == id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur != null)
            {
                _context.Utilisateurs.Remove(utilisateur);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilisateurExists(int id)
        {
            return _context.Utilisateurs.Any(e => e.UtilisateurId == id);
        }
    }
}
