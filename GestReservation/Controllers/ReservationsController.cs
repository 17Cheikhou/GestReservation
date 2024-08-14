using Microsoft.AspNetCore.Mvc;
using GestReservation.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ReservationsController : Controller
{
    private readonly GestReservationContext _context;

    public ReservationsController(GestReservationContext context)
    {
        _context = context;
    }

    // GET: Reservations
    public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 5, int trajetPage = 1, int trajetPageSize = 3)
    {
        ViewData["CurrentFilter"] = searchString;

        var trajetsQuery = _context.Trajets.AsQueryable();
        var reservationsQuery = _context.Reservations.Include(r => r.Trajet).Include(r => r.Utilisateur).AsQueryable();

        if (!String.IsNullOrEmpty(searchString))
        {
            trajetsQuery = trajetsQuery.Where(t => t.VilleArrive.Contains(searchString) || t.VilleDepart.Contains(searchString));
            reservationsQuery = reservationsQuery.Where(r => r.Trajet.VilleDepart.Contains(searchString) || r.Trajet.VilleArrive.Contains(searchString) || r.Utilisateur.Nom.Contains(searchString));
        }

        trajetsQuery = trajetsQuery.OrderBy(t => t.TrajetId);
        reservationsQuery = reservationsQuery.OrderBy(r => r.ReservationId);

        var paginatedTrajets = await PaginatedList<Trajet>.CreateAsync(trajetsQuery, trajetPage, trajetPageSize);
        var paginatedReservations = await PaginatedList<Reservation>.CreateAsync(reservationsQuery, pageNumber, pageSize);

        var viewModel = new ReservationIndexViewModel
        {
            Trajets = paginatedTrajets,
            Reservations = paginatedReservations
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Reserve(int trajetId)
    {
        var trajet = await _context.Trajets.FindAsync(trajetId);
        if (trajet == null)
        {
            return NotFound();
        }

        var utilisateurId = 6; // Remplacez par l'ID de l'utilisateur connecté

        var existingReservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.TrajetId == trajetId && r.UtilisateurId == utilisateurId);

        if (existingReservation != null)
        {
            TempData["ErrorMessage"] = "Vous avez deja reserver ce trajet.";
            return RedirectToAction(nameof(Index));
        }

        var reservation = new Reservation
        {
            TrajetId = trajetId,
            UtilisateurId = utilisateurId,
            DateReservation = DateTime.Now
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Trajet)
            .Include(r => r.Utilisateur)
            .FirstOrDefaultAsync(m => m.ReservationId == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
