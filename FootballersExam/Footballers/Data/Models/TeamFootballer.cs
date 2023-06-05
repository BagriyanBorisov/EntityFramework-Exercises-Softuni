using System.ComponentModel.DataAnnotations.Schema;

namespace Footballers.Data.Models
{
    public class TeamFootballer
    {
        public int TeamId { get; set; }
        public int FootballerId { get; set; }

        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; } = null!;

        [ForeignKey(nameof(FootballerId))]
        public virtual Footballer Footballer { get; set; } = null!;


    }
}
