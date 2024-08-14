using System;
using System.Collections.Generic;

namespace GestReservation.Models;

public partial class Trajet
{
    public int TrajetId { get; set; }

    public string? VilleDepart { get; set; }

    public string? VilleArrive { get; set; }

    public DateTime? DateDepart { get; set; }

    public int? UtilisateurId { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Utilisateur? Utilisateur { get; set; }
}
