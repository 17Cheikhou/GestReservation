using System;
using System.Collections.Generic;

namespace GestReservation.Models;

public partial class Utilisateur
{
    public int UtilisateurId { get; set; }

    public string? Prenom { get; set; }

    public string? Nom { get; set; }

    public string? Email { get; set; }

    public string? MotDePass { get; set; }

    public string? Roles { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Trajet> Trajets { get; set; } = new List<Trajet>();

    public virtual ICollection<Vehicule> Vehicules { get; set; } = new List<Vehicule>();
}
