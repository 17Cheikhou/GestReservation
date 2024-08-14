using System;
using System.Collections.Generic;

namespace GestReservation.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int? UtilisateurId { get; set; }

    public int? TrajetId { get; set; }

    public DateTime? DateReservation { get; set; }

    public virtual Trajet? Trajet { get; set; }

    public virtual Utilisateur? Utilisateur { get; set; }
}
