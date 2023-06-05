using System.ComponentModel.DataAnnotations.Schema;

namespace Trucks.Data.Models
{
    public class ClientTruck
    {
        [ForeignKey(nameof(Truck))]
        public int TruckId { get; set; }

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        public virtual Client Client { get; set; } = null!;
        public virtual Truck Truck { get; set; } = null!;
    }
}
