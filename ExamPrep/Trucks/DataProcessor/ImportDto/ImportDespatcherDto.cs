using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Despatcher")]
    public class ImportDespatcherDto
    {
        [XmlElement("Name")]
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; }

        [XmlElement("Position")]
        [Required(AllowEmptyStrings = false)]
        public string Position { get; set; }

        [XmlArray("Trucks")]
        public ImportTruckDto[] Trucks { get; set; }
    }
}
