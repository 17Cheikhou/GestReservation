using System;
using System.Collections.Generic;

namespace GestReservation.Models;

public partial class Vehicule
{
    public int VehiculeId { get; set; }

    public string? Marque { get; set; }

    public string? Modele { get; set; }

    public string? Immatriculation { get; set; }

    public int? UtilisateurId { get; set; }

    public string? Image { get; set; }

    public virtual Utilisateur? Utilisateur { get; set; }
}
