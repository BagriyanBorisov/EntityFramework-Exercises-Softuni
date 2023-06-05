using Boardgames.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Boardgame")]
    public class ImportBoardgameDto
    {

        [XmlElement("Name")]
        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        public string Name { get; set; } = null!;


        [XmlElement("Rating")]
        [Range(1,10)]
        public double Rating { get; set; }

        [XmlElement("YearPublished")]
        [Range(2018, 2023)]
        public int YearPublished { get; set; }

        [XmlElement("CategoryType")]
        [Range(0,4)]
        public int CategoryType { get; set; }

        [XmlElement("Mechanics")]
        [Required]
        public string? Mechanics { get; set; }
    }
}
