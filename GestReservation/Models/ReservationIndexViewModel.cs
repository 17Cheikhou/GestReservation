namespace GestReservation.Models
{

public class ReservationIndexViewModel
    {
        public PaginatedList<Trajet> Trajets { get; set; }
        public PaginatedList<Reservation> Reservations { get; set; }
    }


}
