using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType("Coach")]
    public class ExportCoachDto
    {
        [XmlAttribute("FootballersCount")]
        public int FootballersCount { get; set; }

        [XmlElement("CoachName")] 
        public string Name { get; set; } = null!;

        [XmlArray("Footballers")]
        public ExportFootballerDto[] Footballers { get; set; }

    }
}
