using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Coach")]
    public class CoachImportDto
    {
      
        [XmlElement("Name")]
        public string? Name { get; set; } 

        [XmlElement("Nationality")]
        public string? Nationality { get; set; }

        [XmlArray("Footballers")]
        [XmlArrayItem("Footballer")]
        public FootballerImportDto[] Footballers { get; set; }

    }
}
